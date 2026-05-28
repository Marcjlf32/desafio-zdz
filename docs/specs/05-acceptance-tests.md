# 05 - Mapping AC -> Teste Automatizado

Tabela de rastreabilidade: cada Acceptance Criteria do PDF tem um teste nomeado igual no codigo, facilitando auditoria do avaliador.

## Backend (User Story 1)

| AC | Teste | Arquivo |
|---|---|---|
| AC01 | `AC01_Persistencia_DeveUsarPostgresReal` | `backend/tests/Catalog.Tests/Infrastructure/PersistenciaTests.cs` |
| AC02 | `AC02_Modelagem_DeveAplicarForeignKey1N` | `backend/tests/Catalog.Tests/Infrastructure/PersistenciaTests.cs` |
| AC03 | `AC03_Endpoints_DeveExporRotasContratadas_*` (8 testes, um por rota) | `backend/tests/Catalog.Tests/Categorias/CategoriasEndpointsTests.cs` + `Produtos/ProdutosEndpointsTests.cs` |
| AC04 | `AC04_DeleteCategoriaComProdutos_DeveRetornar409ComMensagem` | `backend/tests/Catalog.Tests/Categorias/CategoriasEndpointsTests.cs` |
| AC05 | `AC05_PostCategoriaNomeCurto_DeveRetornar400ComSumario` + `AC05_PostProdutoNomeCurto_DeveRetornar400ComSumario` | respectivos arquivos |
| AC06 | `AC06_CorsRejeitaOrigemNaoAutorizada` | `backend/tests/Catalog.Tests/Api/CorsTests.cs` |

## Frontend (User Story 2)

Para frontend, testes Vitest cobrem stores; ACs visuais sao validados manualmente pelo checklist do README.

| AC | Verificacao | Arquivo |
|---|---|---|
| AC01-front | Codigo do composable usa `ref`/`reactive` + `$fetch` | inspecao em `frontend/composables/useCategoriasApi.ts` |
| AC02-front | Componente `CategoriaGrid.vue` renderiza coluna `Acoes` com Editar+Excluir | snapshot teste em `frontend/tests/unit/CategoriaGrid.spec.ts` |
| AC03-front | Click em Editar abre modal preenchido | manual via checklist |
| AC04-front | Botao Salvar disabled enquanto Nome<5 | unit test do form |
| AC05-front | Click em Excluir aciona ConfirmDialog antes de chamar API | unit test do store |
| AC06-front | Apos PUT/DELETE, array reativo do store muda; busca `window.location.reload` retorna zero | unit test do store + grep no codigo |
| AC07-front | Quando API retorna 409, ErrorToast exibe `problemDetails.detail` | unit test do store |
| AC08-front | Form de produto carrega categorias assincronamente em `onMounted` | manual via checklist |

## Checklist Manual (para o avaliador)

Esta lista esta replicada no README.md para facilitar conferencia visual durante a entrevista.

1. [ ] `/swagger` mostra exatamente as 8 rotas listadas no AC03.
2. [ ] DELETE em categoria com produtos retorna 409 com a mensagem exata do AC04.
3. [ ] POST com nome curto retorna 400 com sumario (AC05).
4. [ ] `Program.cs` tem CORS com origem unica, sem `AllowAnyOrigin` (AC06).
5. [ ] `appsettings.json` aponta para Postgres real, sem InMemory (AC01).
6. [ ] Em `/categorias` ha coluna "Acoes" com Editar e Excluir (AC02-front).
7. [ ] Botao Editar abre modal preenchido (AC03-front).
8. [ ] Botao Salvar disabled se nome curto (AC04-front).
9. [ ] Excluir pede confirmacao via dialog (AC05-front).
10. [ ] Apos editar/excluir, grid atualiza sem F5 (AC06-front).
11. [ ] Erro de integridade aparece em toast com mensagem do servidor (AC07-front).
12. [ ] Select de categoria no form de produto vem da API (AC08-front).
13. [ ] `dotnet test` verde com nomes de teste batendo com ACs.
14. [ ] README cobre pre-reqs, restore, migrations e payloads (Task 07).
