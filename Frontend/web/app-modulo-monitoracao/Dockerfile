FROM node:20-alpine AS builder

WORKDIR /app

# Instala o pnpm globalmente conforme a regra
RUN npm install -g pnpm@10.x

# Copia os arquivos de dependência
COPY package.json pnpm-lock.yaml ./

# Instala as dependências
RUN pnpm install --frozen-lockfile

# Copia o resto do código
COPY . .

# Faz o build de produção do Angular
RUN pnpm run build

# Stage 2: NGINX para servir os arquivos estáticos
FROM nginx:alpine

# Remove a página default do nginx
RUN rm -rf /usr/share/nginx/html/*

# Copia o build do Angular (ajustar pasta dist conforme angular.json)
COPY --from=builder /app/dist/app-modulo-monitoracao/browser /usr/share/nginx/html

# Copia arquivo de configuração do Nginx para Single Page Apps (opcional/padrão)
# COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
