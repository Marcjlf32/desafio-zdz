# 01 - Visao

## Epic

**Modernizacao e Escalabilidade da Plataforma de Inventario Comercial**

A operacao atual depende de processos manuais e planilhas descentralizadas para gerenciar o catalogo de mercadorias. Esta entrega centraliza e automatiza a gestao de produtos e categorias em uma plataforma web segura e reativa.

## Valor de Negocio

- Mitigar erros de estoque causados por planilhas paralelas.
- Reduzir o tempo de cadastro de novos itens em 40 por cento.
- Fornecer base de dados integra e relacional para futuras integracoes com modulos de vendas e faturamento.

## Metricas de Sucesso

1. API RESTful capaz de processar operacoes CRUD completas com tempo de resposta inferior a 200ms por requisicao.
2. Zero inconsistencias de orfaos no banco de dados atraves de integridade referencial rigida.
3. Interface reativa que dispense recarregamento total da pagina durante a manutencao de dados.

## Escopo desta entrega

Modulo de Gestao de Catalogo cobrindo Produtos e Categorias, com:

- Back-end .NET expondo API RESTful contra banco relacional permanente.
- Front-end Nuxt 3 com grid central de acoes para manipulacao inline ou via modal.
- Validacoes de negocio aplicadas em ambas as camadas.
- Politica explicita de CORS limitando a comunicacao a origem conhecida.

## Fora de escopo

- Autenticacao e autorizacao (nao solicitadas pelo desafio).
- Modulo de vendas e faturamento (futura integracao mencionada).
- Internacionalizacao do front-end (apenas pt-BR).
- Deploy em nuvem.
