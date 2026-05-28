import { defineStore } from 'pinia'
import type { CategoriaDto, CreateCategoriaDto, UpdateCategoriaDto } from '~/types/api'
import { extractErrorDetail } from '~/utils/extractErrorDetail'

interface CategoriasState {
  items: CategoriaDto[]
  loading: boolean
  error: string | null
}

export const useCategoriasStore = defineStore('categorias', {
  state: (): CategoriasState => ({
    items: [],
    loading: false,
    error: null
  }),

  actions: {
    async fetchAll() {
      const api = useCategoriasApi()
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

    async create(dto: CreateCategoriaDto) {
      const api = useCategoriasApi()
      try {
        const criada = await api.create(dto)
        this.items = [...this.items, criada]
        return criada
      } catch (err) {
        const detail = extractErrorDetail(err)
        this.error = detail
        throw new Error(detail)
      }
    },

    async update(id: number, dto: UpdateCategoriaDto) {
      const api = useCategoriasApi()
      try {
        await api.update(id, dto)
        this.items = this.items.map(c =>
          c.id === id ? { ...c, nome: dto.nome, descricao: dto.descricao } : c
        )
      } catch (err) {
        const detail = extractErrorDetail(err)
        this.error = detail
        throw new Error(detail)
      }
    },

    async remove(id: number) {
      const api = useCategoriasApi()
      try {
        await api.remove(id)
        this.items = this.items.filter(c => c.id !== id)
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
