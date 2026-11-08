# Fase 4: Inteligência Artificial e Visão Computacional 🚀

Concluímos com sucesso a implementação base para análise inteligente de vídeo das nossas câmeras. O projeto `app-worker-captura-video` foi compilado sem erros e agora conta com o módulo que aplica Computer Vision sobre as imagens extraídas pelo FFmpeg.

## O que foi desenvolvido

Para garantir que a integração aconteça sem travar a thread de gravação (que cria os `.mp4`), utilizamos injeção de dependência nativa com rotinas em *Background* puras.

### 1. Extração Dinâmica de Imagens (FrameExtractionService)
Adicionamos um serviço que solicita proativamente ao FFmpeg um "snapshot" rápido da câmera (`-vframes 1`), ignorando o áudio e mantendo apenas a imagem (`.jpg`).
Este frame é imediatamente carregado na memória como bytes (`byte[]`) e o arquivo temporário é destruído para evitar que o HD do servidor lote.

### 2. Motor de IA (VisionAiAnalyzer)
> [!NOTE]  
> Atualmente este serviço está injetado com um `DummyVisionAiAnalyzer` (Simulador de IA), para podermos validar os disparos de rede.

O serviço de IA implementa a interface estrita `IVisionAiAnalyzer`, que aguarda um *buffer* de bytes. Quando receber o provedor real (OpenCV / Azure / ML.NET), ele usará esses bytes para descobrir objetos e pessoas em cena, gerando um evento estruturado de alarme (com Tipo e percentual de Confiança).

### 3. Comunicação com a API (EventosApiClient)
Criamos um cliente HTTP, injetado via `IHttpClientFactory`, que tem a responsabilidade única de despachar as detecções da Inteligência Artificial para a nossa API (`POST /api/v1/eventos/alarme`). Essa modularização mantém o Worker limpo e respeita a separação da Arquitetura Limpa.

### 4. O Robô Orquestrador (VisionAiWorker)
Adicionamos o `VisionAiWorker` no `Program.cs`. Esse Background Service percorre as Câmeras Ativas a cada **5 segundos** (pode ser customizado no `appsettings.json`), extrai a foto, joga para a IA avaliar e, se houver perigo ou uma detecção explícita, repassa à API Central.

---

> [!TIP]  
> Você pode acompanhar o arquivo de Log Centralizado no Kibana (`Core_Logs`) para visualizar os alertas de: "IA detectou 'Pessoa' na câmera X. Confiança: 95%".

## Próximos Passos (Fase 5)
A Fase 4 da fundação está resolvida e pronta para plugar uma API local de IA ou serviço em Cloud. 
Com as detecções enviando as informações para a Base de Dados com sucesso, a próxima etapa será **A Fase 5: Interface do Usuário (Frontend Angular)**, para exibir essas marcações em tela e permitir visualização do vídeo HLS ao vivo.

Se quiser alterar alguma coisa sobre a frequência de fotos (Intervalo) ou testar localmente, me avise!
