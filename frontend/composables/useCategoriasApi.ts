import type { CategoriaDto, CreateCategoriaDto, UpdateCategoriaDto } from '~/types/api'

export const useCategoriasApi = () =>
  useCrudApi<CategoriaDto, CreateCategoriaDto, UpdateCategoriaDto>('/api/categorias')
