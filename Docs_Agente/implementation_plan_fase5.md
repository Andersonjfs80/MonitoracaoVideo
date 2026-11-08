# Fase 5: Interface do Usuário (Frontend Angular)

Este plano descreve como vamos estruturar o projeto Frontend em Angular para consumir a API de Monitoração de Vídeo e visualizar as streams HLS em tempo real, além de gerenciar o cadastro de câmeras.

## User Review Required

> [!IMPORTANT]  
> 1. **Player de Vídeo HLS**: O navegador puro (exceto Safari) não reproduz `.m3u8` nativamente. Recomendo utilizarmos a biblioteca `hls.js` ou `video.js` instalada no Angular. Você concorda com o uso do `hls.js`?
> 2. **Design System**: Conforme as diretrizes, usaremos CSS puro/SCSS com design moderno (Glassmorphism, Modo Escuro nativo). Podemos aplicar Angular Material apenas para o formulário de CRUD?

## Open Questions

> [!WARNING]  
> Qual será a **Porta Docker** alocada para este Pod no ambiente local? No padrão vi portas como 4201 e 4202. Posso configurar este Pod (`app-modulo-monitoracao`) para rodar na porta **4205**?

## Proposed Changes

Abaixo detalho a estrutura do Pod seguindo fielmente as regras estritas da `Padrao_Angular_Frontend_Agente_IA.md`:

### 1. Novo Pod: `app-modulo-monitoracao`
- **Localização:** `Frontend/web/app-modulo-monitoracao`.
- **Framework:** Angular 18+ gerado via CLI.
- **Gerenciador de Pacotes:** `pnpm` (conforme as regras).

### 2. Estrutura Interna (Sem abreviações)
O projeto utilizará a pasta `modulos` no lugar de `features`. 

#### Módulo de Câmeras (CRUD)
- `src/app/modulos/cameras/`
- Componentes Standalone roteados no AppRoutingModule.
- Telas para Listagem, Criação, Edição e Exclusão (`Nome`, `UrlRtsp`, `Ativa`).

#### Módulo de Visualização (Mosaico)
- `src/app/modulos/mosaico/`
- Renderiza uma grid de `N x N` players de vídeo simultâneos consumindo o endpoint HTTP gerado pelo Gateway/StreamManager (ex: `/hls/{id}/stream.m3u8`).

#### Módulo de Histórico (Timeline)
- `src/app/modulos/timeline/`
- Exibirá a linha do tempo dos vídeos MP4 gravados na Fase 3 e os marcadores de Eventos gerados pela IA na Fase 4 (Movimento, Veículo, Pessoa).

### 3. Integração e Segurança
- `src/app/services/api-client.service.ts`: Comunicação centralizada com o `Gateway` (assumindo localhost:5000) injetando o Header obrigatório `X-Sec-Key: 1`.
- Uso restrito de `strict: true` (TypeScript), sem uso de `any`.

## Verification Plan

### Testes Manuais (Workflow)
1. Rodar `pnpm install` e `pnpm start` no novo pod.
2. Acessar `http://localhost:4205`.
3. Validar a criação de uma Câmera no CRUD (Verificando banco SQL).
4. Abrir a tela de Mosaico e validar se o player `hls.js` começa a reproduzir o stream ao vivo.
