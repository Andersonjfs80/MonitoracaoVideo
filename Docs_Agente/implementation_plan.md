# Fase 4: Inteligência Artificial e Visão Computacional (LPR/Detecção)

Este plano descreve como vamos implementar o motor de análise inteligente sobre o streaming RTSP, extraindo screenshots com o FFmpeg e rodando inferência de Inteligência Artificial para detectar Movimentos, Objetos ou Placas Veiculares, e enviando esses eventos de volta para a API Backend.

## User Review Required

> [!IMPORTANT]  
> Precisamos definir qual **Motor de Inteligência Artificial / Visão Computacional** vamos utilizar. No plano inicial não estava especificada a stack de IA. Opções comuns:
> - **OpenCV + Yolo (On-Premise via ML.NET ou Python Wrapper)** (Recomendado para rodar localmente sem custo).
> - **Azure Computer Vision / Custom Vision** (Fácil integração mas tem custo por requisição).
> - **AWS Rekognition**.
>
> Para iniciarmos e validarmos a arquitetura, implementarei um **Mock/Dummy Engine** e o Serviço de Extração, que poderá ser substituído futuramente pela biblioteca real assim que você decidir a stack de IA.

## Open Questions

> [!WARNING]  
> 1. **Frequência de Análise:** A cada quantos segundos devemos extrair e analisar um frame (screenshot) das câmeras ativas? (Sugestão inicial: a cada **5 segundos** por câmera para não sobrecarregar a CPU).
> 2. **Comunicação com o EventosController:** O Worker está no mesmo domínio, porém o `EventosController` é consumido via HTTP (porta da API). A URL da API base deverá estar no `appsettings.json` do Worker, correto?

## Proposed Changes

Abaixo detalho as alterações propostas na nossa arquitetura baseadas na Clean Architecture e Padrões da Agente IA:

### app-worker-captura-video (Worker Service)

Implementaremos um serviço que, em paralelo à gravação contínua em segmentos HLS/MP4, executa FFmpeg periodicamente para extrair 1 frame (imagem .jpg) ou usamos um pipe do FFmpeg. A abordagem de extração pontual é menos intensiva que extrair *todos* os frames.

#### [NEW] [Services/FrameExtractionService.cs](file:///e:/Projetos/MonitoracaoVideo/Backend/app-worker-captura-video/Services/FrameExtractionService.cs)
- Serviço responsável por invocar o `ffmpeg` extraindo exatamente um quadro atualizado via RTSP (`-vframes 1 -q:v 2`) e retornando a imagem em disco ou em memória.

#### [NEW] [Services/VisionAiAnalyzer.cs](file:///e:/Projetos/MonitoracaoVideo/Backend/app-worker-captura-video/Services/VisionAiAnalyzer.cs)
- Implementará uma interface `IVisionAiAnalyzer`.
- O método `AnalyzeImageAsync(byte[] imageBytes)` receberá a imagem capturada e passará pelo motor de IA.
- Retornará um objeto com detalhes do que foi detectado: Tipo (`Movimento`, `Pessoa`, `Placa`), Confiança (%), e Timestamp.

#### [NEW] [Services/EventosApiClient.cs](file:///e:/Projetos/MonitoracaoVideo/Backend/app-worker-captura-video/Services/EventosApiClient.cs)
- Serviço `HttpClient` tipado e gerenciado pelo `IHttpClientFactory`.
- Responsável por enviar a requisição `POST /api/v1/eventos/alarme` para a API Backend (Gateway/Api), seguindo os padrões requeridos.

#### [NEW] [VisionAiWorker.cs](file:///e:/Projetos/MonitoracaoVideo/Backend/app-worker-captura-video/VisionAiWorker.cs)
- Novo `BackgroundService` em paralelo ao `RtspCaptureWorker`.
- Lida com o loop que a cada *X* segundos percorre as `Cameras` ativas no banco de dados, utiliza o `FrameExtractionService` para obter o frame, envia ao `VisionAiAnalyzer` e, caso uma detecção ocorra, usa `EventosApiClient` para salvar o alarme via API.

#### [MODIFY] [Program.cs](file:///e:/Projetos/MonitoracaoVideo/Backend/app-worker-captura-video/Program.cs)
- Registrar os novos serviços (`FrameExtractionService`, `VisionAiAnalyzer`).
- Configurar o `HttpClient` para `EventosApiClient` apontando para a base URL da API (usando `appsettings.json` de acordo com os padrões).
- Registrar o novo `HostedService` (`VisionAiWorker`).

## Verification Plan

### Automated Tests
- *(A depender se temos projeto de testes na solution, criaremos testes unitários para a lógica do mock do `VisionAiAnalyzer` e extração).*

### Manual Verification
1. Rodar a API Backend.
2. Rodar o Worker `app-worker-captura-video`.
3. Adicionar uma câmera RTSP válida (ou simulador HLS/RTSP) no banco de dados com `Ativa = true`.
4. Observar no Console do Worker os logs informando a extração do frame e a análise (Mock) indicando detecção.
5. Observar o log do `EventosController` da API processando o Request HTTP de alarme.
