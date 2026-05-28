# Architectural Decision Records

Decisoes tecnicas relevantes desta entrega. Cada secao segue formato curto: contexto, decisao, consequencia.

---

## ADR 001: PostgreSQL como banco relacional

**Contexto.** O PDF aceita SQL Server, PostgreSQL ou MySQL. Precisamos escolher um que seja leve para o avaliador rodar localmente e familiar para devs .NET modernos.

**Decisao.** Usar PostgreSQL 16 via container Docker durante desenvolvimento. Provider Npgsql no EF Core. Connection string aponta para `localhost:5432`.

**Consequencia.** Avaliador precisa ou rodar `docker run` (1 linha no README) ou instalar Postgres nativamente. Trade-off aceito para nao depender de instalacao de SQL Server (mais pesado no Windows). Para trocar para SQL Server, basta alterar provider em `Program.cs` e connection string.

---

## ADR 002: Clean Architecture com 3 projetos

**Contexto.** O escopo (CRUD de duas entidades) nao justifica 5+ projetos, mas separar em camadas demonstra maturidade e atende SOLID (Dependency Inversion).

**Decisao.** Tres projetos:

- `Catalog.Api` - apresentacao (controllers, filtros, `Program.cs`).
- `Catalog.Application` - regras de aplicacao, DTOs, validators, services.
- `Catalog.Infrastructure` - persistencia, entidades de banco, configurations EF Core.

Dependencias: `Api` -> `Application` -> nada; `Api` -> `Infrastructure`; `Infrastructure` -> `Application`. Entidades vivem em `Infrastructure` porque sao detalhes de persistencia; DTOs em `Application` porque sao contrato com o exterior.

**Consequencia.** Compilavel sem ferramenta externa. Substituir o ORM ou expor outra API (gRPC, GraphQL) seria localizado em poucos arquivos.

---

## ADR 003: Result Pattern para fluxos de negocio

**Contexto.** O cenario do AC04 (delete-com-conflito) e fluxo esperado, nao excecao. Lancar exception para isso polui logs e degrada performance.

**Decisao.** Application Services retornam `Result<T>` com estados explicitos: `Success`, `NotFound`, `Conflict(detail)`, `ValidationFailed`. Controllers fazem mapping para HTTP status codes.

**Consequencia.** Exceptions ficam reservadas para erros de infra (DB fora, IO, etc.). Testes ficam mais claros: assert sobre estado do Result em vez de pegar exception.

---

## ADR 004: ProblemDetails RFC 7807 para erros

**Contexto.** Front-end precisa exibir mensagens amigaveis (AC07-front). Sem padrao, cada endpoint inventaria seu shape de erro.

**Decisao.** Adotar `application/problem+json` (RFC 7807) em todos os erros. Validacao usa `ValidationProblemDetails` (extensao com campo `errors`). Conflict usa `ProblemDetails` simples com `title` e `detail`.

**Consequencia.** Frontend consome shape padrao: `error.data.detail` para toast (AC07-front). Documentacao automatica via Swagger fica consistente.

---

## ADR 005: Tipos do frontend gerados da OpenAPI

**Contexto.** Manter tipos manualmente sincronizados entre back e front gera drift. Quando o avaliador rodar, qualquer divergencia salta.

**Decisao.** Versionar `docs/specs/04-api-contract.yaml` como fonte da verdade. Gerar `frontend/types/api.generated.ts` via `openapi-typescript`. Backend implementa conforme a spec.

**Consequencia.** Build do frontend quebra ao usar campo inexistente na spec. Mudancas no contrato exigem update da spec primeiro, depois codigo (spec-driven). Comando: `npx openapi-typescript ../docs/specs/04-api-contract.yaml -o types/api.generated.ts`.

---

## ADR 006: CORS com origem unica explicita

**Contexto.** AC06 proibe `AllowAnyOrigin`. Em dev usamos `http://localhost:3000` (Nuxt default).

**Decisao.** Politica nomeada `FrontendOnly` em `Program.cs`, com `WithOrigins("http://localhost:3000")` lida de `appsettings.json` (campo `Cors:AllowedOrigins`).

**Consequencia.** Para mudar a origem do frontend, edita-se `appsettings.json`, sem recompilar. Teste automatizado verifica que outra origem retorna sem header `Access-Control-Allow-Origin`.

---

## ADR 007: Sem comentarios em codigo, comentarios so em docs

**Contexto.** Comentarios em codigo envelhecem mal e poluem leitura. Esta entrega adota convencao do usuario (Marcelo): zero comentarios em codigo, documentacao consolidada em `docs/`.

**Decisao.** Nenhum arquivo `.cs`, `.ts`, ou `.vue` contem comentarios. Decisoes vao em ADRs; intent fica em `docs/specs/`; testes documentam comportamento via nome (`AC04_Delete...`).

**Consequencia.** Codigo precisa ser auto-explicativo. Nomes longos sao preferiveis a abreviacoes com comentario.
