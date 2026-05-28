import { defineStore } from 'pinia'
import type { ProdutoDto, CreateProdutoDto, UpdateProdutoDto } from '~/types/api'
import { extractErrorDetail } from '~/utils/extractErrorDetail'

interface ProdutosState {
  items: ProdutoDto[]
  loading: boolean
  error: string | null
}

export const useProdutosStore = defineStore('produtos', {
  state: (): ProdutosState => ({
    items: [],
    loading: false,
    error: null
  }),

  actions: {
    async fetchAll() {
      const api = useProdutosApi()
      this.loading = true
      this.error = null
      try {
        this.items = await api.list()
      } catch (err) {
        this.error = extractErrorDetail(err)
      } finally {
        this.loading = false
      }
    },

    async create(dto: CreateProdutoDto) {
      const api = useProdutosApi()
      try {
        const criado = await api.create(dto)
        this.items = [...this.items, criado]
        return criado
      } catch (err) {
        const detail = extractErrorDetail(err)
        this.error = detail
        throw new Error(detail)
      }
    },

    async update(id: number, dto: UpdateProdutoDto) {
      const api = useProdutosApi()
      const categorias = useCategoriasStore()
      try {
        await api.update(id, dto)
        const categoria = categorias.items.find(c => c.id === dto.categoriaId)
        this.items = this.items.map(p =>
          p.id === id
            ? {
                ...p,
                nome: dto.nome,
                descricao: dto.descricao,
                preco: dto.preco,
                categoriaId: dto.categoriaId,
                categoria: categoria ?? p.categoria
              }
            : p
        )
      } catch (err) {
        const detail = extractErrorDetail(err)
        this.error = detail
        throw new Error(detail)
      }
    },

    async remove(id: number) {
      const api = useProdutosApi()
      try {
        await api.remove(id)
        this.items = this.items.filter(p => p.id !== id)
      } catch (err) {
        const detail = extractErrorDetail(err)
        this.error = detail
        throw new Error(detail)
      }
    },

    clearError() {
      this.error = null
    }
  }
})
