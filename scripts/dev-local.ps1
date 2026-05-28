$ErrorActionPreference = "Stop"

# Script de dev local para o Marcelo - usa portas alternativas
# para evitar conflitos com o ambiente Windows + outros containers.
#
# Uso: .\scripts\dev-local.ps1 [api|front|tests|all]
# Default: all (sobe Postgres, API e Frontend em terminais separados)

param(
    [Parameter(Position = 0)]
    [ValidateSet("api", "front", "tests", "all")]
    [string]$Target = "all"
)

$ApiPort = 5050
$DbPort = 5433
$FrontPort = 3000
$DbName = "catalog"
$DbPassword = "dev"
$Container = "catalog-pg"

$ConnApi = "Host=localhost;Port=$DbPort;Database=$DbName;Username=postgres;Password=$DbPassword"
$ConnTest = "Host=localhost;Port=$DbPort;Database=catalog_test;Username=postgres;Password=$DbPassword"

$env:ASPNETCORE_URLS = "http://localhost:$ApiPort"
$env:Cors__AllowedOrigins__0 = "http://localhost:$FrontPort"
$env:NUXT_PUBLIC_API_BASE_URL = "http://localhost:$ApiPort"

function Start-Postgres {
    $existing = docker ps -a --filter "name=^$Container$" --format "{{.Names}}"
    if ($existing -eq $Container) {
        Write-Host "Container '$Container' ja existe. Iniciando..."
        docker start $Container | Out-Null
    }
    else {
        Write-Host "Criando container '$Container' na porta $DbPort..."
        docker run --name $Container `
            -e POSTGRES_PASSWORD=$DbPassword `
            -e POSTGRES_DB=$DbName `
            -p "${DbPort}:5432" `
            -d postgres:16 | Out-Null
    }

    Write-Host "Aguardando Postgres aceitar conexoes..."
    for ($i = 0; $i -lt 30; $i++) {
        docker exec $Container pg_isready -U postgres 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Postgres pronto em localhost:$DbPort"
            return
        }
        Start-Sleep -Seconds 1
    }
    throw "Timeout aguardando Postgres."
}

function Start-Api {
    Push-Location "$PSScriptRoot\..\backend"
    try {
        $env:ConnectionStrings__CatalogDb = $ConnApi
        Write-Host "Aplicando migrations em $ConnApi"
        dotnet ef database update --project src/Catalog.Infrastructure --startup-project src/Catalog.Api
        Write-Host "Subindo API em $env:ASPNETCORE_URLS"
        dotnet run --project src/Catalog.Api
    }
    finally {
        Pop-Location
    }
}

function Start-Front {
    Push-Location "$PSScriptRoot\..\frontend"
    try {
        if (-not (Test-Path "node_modules")) {
            Write-Host "Instalando dependencias do frontend..."
            npm install
        }
        Write-Host "Subindo Nuxt em http://localhost:$FrontPort"
        npm run dev
    }
    finally {
        Pop-Location
    }
}

function Start-Tests {
    Push-Location "$PSScriptRoot\..\backend"
    try {
        $env:ConnectionStrings__CatalogDb = $ConnTest
        Write-Host "Rodando testes contra Postgres em $ConnTest"
        dotnet test
    }
    finally {
        Pop-Location
    }
}

Start-Postgres

switch ($Target) {
    "api" { Start-Api }
    "front" { Start-Front }
    "tests" { Start-Tests }
    "all" {
        Write-Host ""
        Write-Host "Postgres rodando em background."
        Write-Host "Para iniciar a API em outro terminal: .\scripts\dev-local.ps1 api"
        Write-Host "Para iniciar o Frontend em outro terminal: .\scripts\dev-local.ps1 front"
        Write-Host "Para rodar os testes: .\scripts\dev-local.ps1 tests"
    }
}
