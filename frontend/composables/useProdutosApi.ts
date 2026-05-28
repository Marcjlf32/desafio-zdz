import type { ProdutoDto, CreateProdutoDto, UpdateProdutoDto } from '~/types/api'

export const useProdutosApi = () =>
  useCrudApi<ProdutoDto, CreateProdutoDto, UpdateProdutoDto>('/api/produtos')
