import type { components } from './api.generated'

export type CategoriaDto = components['schemas']['CategoriaDto']
export type CreateCategoriaDto = components['schemas']['CreateCategoriaDto']
export type UpdateCategoriaDto = components['schemas']['UpdateCategoriaDto']

export type ProdutoDto = components['schemas']['ProdutoDto']
export type CreateProdutoDto = components['schemas']['CreateProdutoDto']
export type UpdateProdutoDto = components['schemas']['UpdateProdutoDto']

export type ProblemDetails = components['schemas']['ProblemDetails']
export type ValidationProblemDetails = components['schemas']['ValidationProblemDetails']

export interface FetchError {
  statusCode?: number
  data?: ProblemDetails
}
