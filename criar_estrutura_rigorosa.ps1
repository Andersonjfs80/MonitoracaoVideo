Set-Location 'e:\Projetos\MonitoracaoVideo'

# Limpar qualquer resquício (ignorando erros se as pastas não existirem)
Remove-Item -Path 'Gateway' -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path 'Worker' -Recurse -Force -ErrorAction SilentlyContinue

# Criar Pastas Raízes exatas solicitadas
New-Item -ItemType Directory -Force -Path 'FRONTEND\web'
New-Item -ItemType Directory -Force -Path 'FRONTEND\mobile'
New-Item -ItemType Directory -Force -Path 'BFF'
New-Item -ItemType Directory -Force -Path 'BACKEND'

# --- CRIAR BACKEND ---
$backendName = "app-backend-monitoracao-video"
$backendPath = "BACKEND\$backendName"
New-Item -ItemType Directory -Force -Path $backendPath
Set-Location $backendPath

dotnet new sln -n $backendName
dotnet new webapi -n "$backendName.api"
dotnet new classlib -n "$backendName.domain"
dotnet new classlib -n "$backendName.infrastructure"

dotnet sln add "$backendName.api\$backendName.api.csproj"
dotnet sln add "$backendName.domain\$backendName.domain.csproj"
dotnet sln add "$backendName.infrastructure\$backendName.infrastructure.csproj"

Set-Location 'e:\Projetos\MonitoracaoVideo'

# --- CRIAR BFF ---
$bffName = "app-bff-monitoracao-video"
$bffPath = "BFF\$bffName"
New-Item -ItemType Directory -Force -Path $bffPath
Set-Location $bffPath

dotnet new sln -n $bffName
dotnet new webapi -n "$bffName.api"
dotnet sln add "$bffName.api\$bffName.api.csproj"

Set-Location 'e:\Projetos\MonitoracaoVideo'

Write-Output "Limpeza e estruturação concluídas no padrão absoluto!"
