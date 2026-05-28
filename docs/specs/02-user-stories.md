# 02 - User Stories

## Feature: Modulo de Gestao de Catalogo (Produtos e Categorias)

Feature mae: Epic - Modernizacao e Escalabilidade da Plataforma de Inventario Comercial.

Desenvolvimento ponta a ponta de um ecossistema funcional de cadastro, listagem, edicao e delecao de Produtos e Categorias. O candidato deve estruturar uma API em .NET acoplada a um banco de dados real e uma interface em Nuxt 3 onde o grid de listagem funciona como a central de acoes (inline ou modal) para manipulacao dos dados.

---

## User Story 1 (Back-end): API RESTful de Persistencia e Ciclo de Vida do Catalogo (.NET)

**Como** Desenvolvedor Backend da aplicacao,
**Quero** expor endpoints estruturados para criacao, leitura, atualizacao e exclusao (CRUD) de produtos e categorias,
**Para que** a camada cliente possa manipular e consumir dados integros, validados e de forma performatica.

### Acceptance Criteria

#### AC01 - Arquitetura e Persistencia

Configurar o Entity Framework Core utilizando exclusivamente um banco relacional permanente (SQL Server, PostgreSQL ou MySQL).

- Variaveis estaticas no codigo nao sao aceitas.
- Provedor `InMemory` do EF Core nao e aceito.

#### AC02 - Modelagem Relacional

Criar as tabelas:

- `Categoria` (Id, Nome, Descricao)
- `Produto` (Id, Nome, Descricao, Preco, CategoriaId)

O mapeamento via Fluent API ou Data Annotations deve explicitar o relacionamento 1 para Muitos (1:N) com Chaves Estrangeiras (FK).

#### AC03 - Contrato de Endpoints

Expor estritamente as rotas:

| Metodo | Rota |
|---|---|
| GET | `/api/categorias` |
| POST | `/api/categorias` |
| PUT | `/api/categorias/{id}` |
| DELETE | `/api/categorias/{id}` |
| GET | `/api/produtos` (deve incluir o objeto aninhado da categoria vinculada utilizando `.Include()`) |
| POST | `/api/produtos` |
| PUT | `/api/produtos/{id}` |
| DELETE | `/api/produtos/{id}` |

#### AC04 - Regra de Integridade Referencial

Ao receber `DELETE /api/categorias/{id}`, o back-end deve verificar se existem produtos associados a ela.

Caso existam, a exclusao deve ser impedida, retornando:

- HTTP `409 Conflict` (ou `400 Bad Request`)
- Mensagem textual: `"Não é possível excluir uma categoria que possua produtos vinculados."`

#### AC05 - Validacao de Payload

Os endpoints POST e PUT devem rejeitar requisicoes em que o campo `Nome` seja nulo ou possua menos de 5 caracteres.

- Resposta: `HTTP 400 Bad Request`
- Corpo: sumario dos erros (formato ProblemDetails RFC 7807).

#### AC06 - Seguranca de Comunicacao (CORS)

Implementar politica explicita de CORS no arquivo `Program.cs`, liberando requisicoes unicamente da URL/porta oficial do projeto frontend Nuxt 3.

- Uso de `.AllowAnyOrigin()` e proibido.

---

## User Story 2 (Front-end): Grid Reativo de Gerenciamento e Manutencao de Dados (Vue 3 / Nuxt 3)

**Como** Operador do sistema de estoque,
**Quero** uma interface fluida que liste os dados e me permita criar, editar ou excluir produtos e categorias diretamente pelo Grid,
**Para** manter o catalogo comercial atualizado com agilidade e sem transicoes desnecessarias de pagina.

### Acceptance Criteria

#### AC01-front - Estado e Comunicacao Assincrona

Gerenciar o formulario e os dados do Grid usando a Composition API (`ref`, `reactive`).

A comunicacao com a API .NET deve utilizar Axios ou o cliente nativo do Nuxt (`$fetch` / `useFetch`).

#### AC02-front - Coluna de Acoes no Grid

A tabela de listagem de categorias e produtos deve possuir obrigatoriamente uma coluna final chamada `Acoes`, contendo dois botoes distintos por linha: `Editar` e `Excluir`.

#### AC03-front - Fluxo de Edicao Dinamica

Ao clicar em `Editar`, a interface deve abrir um componente de dialogo/modal preenchido com as informacoes atuais daquela linha, ou transformar a linha do grid em campos editaveis (inline).

O salvamento deve disparar um `PUT` para a API.

#### AC04-front - Validacao em Tela

O botao `Salvar` deve permanecer desativado (`disabled`) enquanto o campo `Nome` nao atender a regra de negocio (minimo de 5 caracteres preenchidos).

#### AC05-front - Confirmacao de Delecao

Ao acionar o botao `Excluir` em uma linha, a interface deve obrigatoriamente interceptar o clique e exibir um alerta de confirmacao.

A requisicao HTTP DELETE so pode ser disparada se o usuario confirmar a intencao.

#### AC06-front - Sincronizacao Sem Refresh

Apos o retorno de sucesso das operacoes de PUT ou DELETE vindas da API, a interface deve atualizar a lista local em memoria de forma reativa instantaneamente.

E proibido recarregar a aba ou a pagina forcadamente.

#### AC07-front - Tratamento Visual de Excecoes

Caso a API rejeite uma exclusao devido a regra de integridade referencial, o front-end deve capturar o erro HTTP e exibir uma mensagem amigavel na tela (toast ou banner) contendo o texto explicativo enviado pelo servidor.

#### AC08-front - Consumo de Componente Vinculado

No formulario de manipulacao de Produtos, a escolha da categoria deve ser feita atraves de um componente do tipo `Select` alimentado assincronamente a partir dos dados do endpoint `GET /api/categorias`.

---

## Tasks Tecnicas (Quebra de Atividades)

| Task | Camada | Descricao |
|---|---|---|
| Task 01 | Back | Inicializar Web API .NET Core + classes de modelo + `appsettings.json` com connection string |
| Task 02 | Back | DbContext + Fluent API (relacionamento 1:N) + Migrations |
| Task 03 | Back | Controllers + CRUD + validacao de nome + tratamento de erro de delete |
| Task 04 | Front | Inicializar Nuxt 3 + rotas para `/categorias` e `/produtos` + layout |
| Task 05 | Front | Formulario de cadastro + Grid consumindo API assincronamente |
| Task 06 | Front | Captura de cliques no Grid + modal de edicao + dialog de confirmacao |
| Task 07 | Doc | README com pre-requisitos, restore, migrations e payloads esperados |
