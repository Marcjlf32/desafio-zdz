# Desafio Técnico ZDZCode - Catálogo Comercial

Solução Full Stack para o desafio de código: CRUD de Produtos e Categorias com .NET 9 + Nuxt 3 + Vuetify 3 + PostgreSQL, desenvolvido com **Spec-Driven Design**.

## Visão Geral

A pasta `docs/` contém a especificação completa antes do código: visão, user stories, modelo de dados, contrato OpenAPI 3.1 e ADRs. O código segue o contrato versionado; tipos do frontend são **gerados** a partir da OpenAPI.

```
desafio-zdz/
|-- docs/                  Especificações e decisões arquiteturais
|   |-- specs/
|   |   |-- 01-vision.md
|   |   |-- 02-user-stories.md
|   |   |-- 03-data-model.md
|   |   |-- 04-api-contract.yaml    (fonte da verdade da API)
|   |   `-- 05-acceptance-tests.md
|   `-- decisions.md
|-- backend/               .NET 9 (Api + Application + Infrastructure + Tests)
|-- frontend/              Nuxt 3 + Vue 3 + Vuetify 3 + Pinia
|-- scripts/
|   `-- start-postgres.ps1
`-- README.md
```

## Pré-requisitos

| Ferramenta | Versão | Como instalar (PowerShell) |
|---|---|---|
| .NET SDK | 9.0+ | `winget install Microsoft.DotNet.SDK.9` |
| Node.js | 20+ | `winget install OpenJS.NodeJS.LTS` |
| PostgreSQL | 16 | Docker (preferencial) ou `winget install PostgreSQL.PostgreSQL.16` |
| Docker Desktop | qualquer | `winget install Docker.DockerDesktop` (opcional, atalho para Postgres) |
| EF Core CLI | 9.0+ | `dotnet tool install --global dotnet-ef` |

## Como Rodar (Passo a Passo)

### 1. Subir o Postgres

Opção A - Docker (recomendado):
```powershell
.\scripts\start-postgres.ps1
```

Opção B - Postgres nativo:
- Crie um database chamado `catalog`.
- Ajuste a connection string em `backend/src/Catalog.Api/appsettings.json`.

### 2. Aplicar Migrations e Subir a API

```powershell
cd backend
dotnet restore
dotnet ef database update --project src/Catalog.Infrastructure --startup-project src/Catalog.Api
dotnet run --project src/Catalog.Api
```

A API sobe em `http://localhost:5000`. Swagger UI: `http://localhost:5000/swagger`.

### 3. Subir o Frontend

Em outro terminal:
```powershell
cd frontend
npm install
npm run dev
```

Frontend em `http://localhost:3000`.

### 4. Rodar os Testes

```powershell
cd backend
dotnet test
```

Os testes usam um banco separado (`catalog_test`) que é dropado e recriado a cada execução - o Postgres precisa estar rodando.

## Contrato da API

8 rotas obrigatórias conforme o desafio (AC03):

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/categorias` | Lista categorias |
| POST | `/api/categorias` | Cria categoria |
| PUT | `/api/categorias/{id}` | Atualiza categoria |
| DELETE | `/api/categorias/{id}` | Exclui categoria (409 se há produtos vinculados) |
| GET | `/api/produtos` | Lista produtos com categoria aninhada (`.Include()`) |
| POST | `/api/produtos` | Cria produto |
| PUT | `/api/produtos/{id}` | Atualiza produto |
| DELETE | `/api/produtos/{id}` | Exclui produto |

### Payloads de exemplo

**POST /api/categorias**
```json
{
  "nome": "Eletrônicos",
  "descricao": "Produtos eletrônicos diversos"
}
```

Resposta `201 Created`:
```json
{
  "id": 1,
  "nome": "Eletrônicos",
  "descricao": "Produtos eletrônicos diversos"
}
```

**POST /api/produtos**
```json
{
  "nome": "Smartphone XYZ",
  "descricao": "Modelo 2026",
  "preco": 2499.90,
  "categoriaId": 1
}
```

Resposta `201 Created`:
```json
{
  "id": 1,
  "nome": "Smartphone XYZ",
  "descricao": "Modelo 2026",
  "preco": 2499.90,
  "categoriaId": 1,
  "categoria": {
    "id": 1,
    "nome": "Eletrônicos",
    "descricao": "Produtos eletrônicos diversos"
  }
}
```

**POST com nome curto retorna `400 Bad Request`**:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Nome": ["O nome deve ter no mínimo 5 caracteres."]
  }
}
```

**DELETE de categoria com produtos vinculados retorna `409 Conflict`**:
```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
  "title": "Conflict",
  "status": 409,
  "detail": "Não é possível excluir uma categoria que possua produtos vinculados."
}
```

## Troubleshooting

### Porta 5432 ou 5000 já ocupada

Se já houver Postgres rodando localmente ou outro serviço ocupando essas portas:

```powershell
# Override via variáveis de ambiente
$env:ASPNETCORE_URLS = "http://localhost:5050"
$env:ConnectionStrings__CatalogDb = "Host=localhost;Port=5433;Database=catalog;Username=postgres;Password=dev"
$env:Cors__AllowedOrigins__0 = "http://localhost:3000"
$env:NUXT_PUBLIC_API_BASE_URL = "http://localhost:5050"

# Subir Postgres em outra porta
docker run --name  -e POSTGRES_PASSWORD=dev -e POSTGRES_DB=catalog -p 5433:5432 -d postgres:16
```

Ou use o script auxiliar que já aplica esses overrides:

```powershell
.\scripts\dev-local.ps1 all     # mostra instruções
.\scripts\dev-local.ps1 api     # sobe API em :5050 contra DB em :5433
.\scripts\dev-local.ps1 front   # sobe Nuxt em :3000 apontando pra :5050
.\scripts\dev-local.ps1 tests   # roda testes contra DB em :5433
```

### Erro "Vite Node IPC socket path not configured"

Em algumas combinações Windows + PowerShell + Node 22, o dev server do Nuxt pode falhar com esse erro ao acessar uma rota. É um problema conhecido de IPC entre processos no Windows.

**Workaround**: gerar build estático e servir como SPA. Funcionalmente idêntico, sem dev hot reload:

```powershell
cd frontend
npm run generate
npx serve .output/public -l 3000
```

Acesse normalmente em `http://localhost:3000`. Todas as funcionalidades funcionam — apenas perde hot reload (que não é necessário para avaliar o app rodando).

## Decisões Arquiteturais

Ver `docs/decisions.md` para ADRs completos. Resumo:

1. **PostgreSQL** como banco relacional (provider Npgsql no EF Core).
2. **3 projetos backend**: Api -> Application -> Infrastructure. Domain entities vivem em Application/Domain; DbContext em Infrastructure implementa `ICatalogDbContext` declarada em Application (inversão de dependência).
3. **Result Pattern** no Application layer: fluxos de negócio (conflict, not found, validation) não usam exceptions.
4. **ProblemDetails RFC 7807** para todas as respostas de erro.
5. **OpenAPI versionada** em `docs/specs/04-api-contract.yaml` como fonte da verdade; tipos do frontend gerados via `npx openapi-typescript`.
6. **CORS explícito** com origem única (`http://localhost:3000`), sem `AllowAnyOrigin` (AC06).
7. **Sem comentários em código** - convenção consciente; documentação consolidada em `docs/`.

## Mapeamento Acceptance Criteria

| AC | Onde foi implementado | Como validar |
|---|---|---|
| AC01 - EF Core + DB real | `appsettings.json` + `CatalogDbContext.cs` | Sem `UseInMemoryDatabase` em nenhum arquivo |
| AC02 - Modelagem 1:N via Fluent API | `Persistence/Configurations/*` | `HasMany().WithOne().HasForeignKey()` em `CategoriaConfiguration` |
| AC03 - Contrato de endpoints | `Controllers/CategoriasController.cs`, `Controllers/ProdutosController.cs` | Swagger lista 8 rotas exatas |
| AC04 - Integridade referencial 409 | `CategoriaService.DeleteAsync` | Teste `AC04_DeleteCategoriaComProdutos_DeveRetornar409ComMensagem` |
| AC05 - Validação nome>=5 | `CategoriaValidator.cs`, `ProdutoValidator.cs` | Testes `AC05_*NomeCurto_DeveRetornar400` |
| AC06 - CORS explícito | `Program.cs` linhas 24-39 | Teste `AC06_CorsRejeitaOrigemNaoAutorizada` |
| AC01-front - Composition API + $fetch | `composables/useCategoriasApi.ts`, `useProdutosApi.ts` | Inspeção do código |
| AC02-front - Coluna Ações | `components/Categoria/CategoriaGrid.vue`, `components/Produto/ProdutoGrid.vue` | Headers têm coluna `actions` com Editar+Excluir |
| AC03-front - Edição via modal | `components/Categoria/CategoriaFormDialog.vue`, `ProdutoFormDialog.vue` | `<v-dialog>` aberto via `@click="openEdit(item)"` |
| AC04-front - Salvar disabled<5 | Form dialogs - `:disabled="!nameIsValid"` | Botão desabilitado enquanto nome curto |
| AC05-front - Confirm de delete | `components/Common/ConfirmDialog.vue` consumido pelos grids | Click em Excluir abre dialog antes de chamar API |
| AC06-front - Sync reativo sem reload | `stores/categorias.store.ts`, `produtos.store.ts` | `grep -r "window.location.reload" frontend/` retorna zero |
| AC07-front - Toast de erro | `components/ErrorToast.vue` | Tenta deletar categoria com produto -> snackbar vermelho |
| AC08-front - Select de categoria | `components/Produto/ProdutoFormDialog.vue` | `<v-select :items="categorias">` carregado no watch |

## Checklist do Avaliador

- [ ] `/swagger` mostra exatamente as 8 rotas do AC03
- [ ] DELETE em categoria com produtos retorna 409 com a mensagem exata
- [ ] POST com `Nome="abc"` retorna 400 com sumário
- [ ] `Program.cs` tem CORS com origem única (sem `AllowAnyOrigin`)
- [ ] `appsettings.json` aponta para Postgres real (sem InMemory)
- [ ] Em `/categorias`: coluna "Ações" com Editar+Excluir
- [ ] Botão Editar abre modal preenchido com dados atuais
- [ ] Botão Salvar disabled se nome curto
- [ ] Excluir pede confirmação via dialog
- [ ] Após editar/excluir, grid atualiza sem F5
- [ ] Erro de integridade aparece em toast com mensagem do servidor
- [ ] Select de categoria no form de produto vem da API
- [ ] `dotnet test` verde com nomes de teste batendo com ACs

## Spec-Driven Design

O projeto foi construído seguindo a ordem:

```
1. Specs (docs/specs/*.md) -> descrevem o "o que" antes do código
2. OpenAPI yaml (04-api-contract.yaml) -> contrato HTTP versionado
3. openapi-typescript -> gera types/api.generated.ts no frontend
4. Backend implementa CONFORMANDO com a OpenAPI
5. Frontend usa os tipos GERADOS (zero drift entre back e front)
6. Testes nomeados igual aos ACs do PDF (rastreabilidade total)
```

Regenerar tipos do frontend após mudar a OpenAPI:
```powershell
cd frontend
npm run types:generate
```

## Stack Completa

**Backend**: .NET 9, ASP.NET Core, Entity Framework Core 9, Npgsql, FluentValidation, Serilog, Swashbuckle, xUnit, FluentAssertions, WebApplicationFactory.

**Frontend**: Nuxt 3, Vue 3 (Composition API), TypeScript strict, Vuetify 3, Pinia, Zod, openapi-typescript.

**Infra**: PostgreSQL 16, Docker (opcional).

## Licença

[MIT](./LICENSE)
