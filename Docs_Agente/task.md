# Mapa Completo do Projeto - Monitoração de Vídeo Inteligente (NVR Vision AI)

## ✅ Fase 1: Fundação e Ambiente
- `[x]` Configurar ambiente Docker (SQL Server `NvrVisionAiDb`, Kibana, Elasticsearch).
- `[x]` Definir padrões de arquitetura (Clean Architecture).
- `[x]` Definir diretrizes rigorosas (Nomenclatura sem abreviações, Injeção Core_Logs).

## ✅ Fase 2: Backend Base (API CRUD)
- `[x]` Criar os 3 projetos C#: API, Domain e Infrastructure.
- `[x]` Configurar o Entity Framework (`MonitoracaoVideoDbContext`).
- `[x]` Criar tabelas principais (`Cameras`, `Gravacoes`, `ReconhecimentoPlacas`).
- `[x]` Integrar a solução corporativa `Core_Logs` (Elasticsearch/Kafka).

## ✅ Fase 3: Core de Captura e Processamento de Vídeo (Worker)
- `[x]` Criar Worker Service dedicado (`app-worker-captura-video`) para gerenciar as gravações.
- `[x]` Implementar comunicação base com FFmpeg.
- `[x]` Implementar rotina de Segmentação e Gravação em Disco (Vídeos MP4 em fatias de 15 min).
- `[x]` Implementar o tradutor RTSP -> HLS sob demanda (StreamManager e StreamController).
- `[x]` Criar endpoint para acionamento de Gravação por Evento/Movimento (EventosController).

## ✅ Fase 4: Inteligência Artificial e Visão Computacional (LPR/Detecção)
A mágica acontece aqui: analisar o stream de vídeo em tempo real ou sob demanda para identificar padrões.
- `[x]` Integrar motor de Reconhecimento de Placas Veiculares (LPR) ou Pessoas.
- `[x]` Criar BackgroundService para analisar screenshots extraídos pelo FFmpeg a cada *X* segundos.
- `[x]` Enviar eventos disparados para o `EventosController` criado na Fase 3.

## ⏳ Fase 5: Interface do Usuário (Frontend Angular)
- `[/]` Configurar projeto Angular seguindo o padrão de UI da empresa.
- `[ ]` Criar grid (mosaico) de visualização das câmeras ao vivo consumindo os arquivos HLS (`.m3u8`).
- `[ ]` Criar tela de CRUD de Câmeras para gerenciar os IPs e URLs RTSP.
- `[ ]` Criar "Timeline" para visualizar vídeos gravados e marcações de movimento.
