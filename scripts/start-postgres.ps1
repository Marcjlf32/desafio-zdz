$ErrorActionPreference = "Stop"

$container = "catalog-pg"
$existing = docker ps -a --filter "name=^$container$" --format "{{.Names}}"

if ($existing -eq $container) {
    Write-Host "Container '$container' ja existe. Iniciando..."
    docker start $container | Out-Null
} else {
    Write-Host "Criando container '$container'..."
    docker run --name $container `
        -e POSTGRES_PASSWORD=dev `
        -e POSTGRES_DB=catalog `
        -p 5432:5432 `
        -d postgres:16 | Out-Null
}

Write-Host "Aguardando Postgres aceitar conexoes..."
$ready = $false
for ($i = 0; $i -lt 30; $i++) {
    $check = docker exec $container pg_isready -U postgres 2>$null
    if ($LASTEXITCODE -eq 0) {
        $ready = $true
        break
    }
    Start-Sleep -Seconds 1
}

if ($ready) {
    Write-Host "Postgres pronto em localhost:5432 (db=catalog, user=postgres, password=dev)."
} else {
    Write-Error "Timeout aguardando Postgres iniciar."
}
