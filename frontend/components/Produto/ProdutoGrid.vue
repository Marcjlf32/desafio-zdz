<script setup lang="ts">
import { storeToRefs } from 'pinia'
import type { ProdutoDto } from '~/types/api'

const store = useProdutosStore()
const { items, loading } = storeToRefs(store)

const formOpen = ref(false)
const confirmOpen = ref(false)
const editing = ref<ProdutoDto | null>(null)
const toRemove = ref<ProdutoDto | null>(null)
const removing = ref(false)

onMounted(() => store.fetchAll())

const openCreate = () => {
  editing.value = null
  formOpen.value = true
}

const openEdit = (produto: ProdutoDto) => {
  editing.value = produto
  formOpen.value = true
}

const askRemove = (produto: ProdutoDto) => {
  toRemove.value = produto
  confirmOpen.value = true
}

const confirmRemove = async () => {
  if (!toRemove.value) return
  removing.value = true
  try {
    await store.remove(toRemove.value.id)
    confirmOpen.value = false
    toRemove.value = null
  } catch {
    confirmOpen.value = false
    toRemove.value = null
  } finally {
    removing.value = false
  }
}

const formatPreco = (valor: number) =>
  valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })

const headers = [
  { title: 'ID', key: 'id', width: 80 },
  { title: 'Nome', key: 'nome' },
  { title: 'Preço', key: 'preco', width: 140 },
  { title: 'Categoria', key: 'categoria.nome' },
  { title: 'Ações', key: 'actions', sortable: false, width: 180 }
]
</script>

<template>
  <v-card>
    <v-card-title class="d-flex align-center">
      <span>Produtos</span>
      <v-spacer />
      <v-btn color="primary" prepend-icon="mdi-plus" @click="openCreate">Novo produto</v-btn>
    </v-card-title>

    <v-data-table
      :headers="headers"
      :items="items"
      :loading="loading"
      density="comfortable"
      no-data-text="Nenhum produto cadastrado."
    >
      <template #item.preco="{ item }">{{ formatPreco(item.preco) }}</template>
      <template #item.actions="{ item }">
        <v-btn icon="mdi-pencil" variant="text" size="small" color="primary" @click="openEdit(item)" />
        <v-btn icon="mdi-delete" variant="text" size="small" color="error" @click="askRemove(item)" />
      </template>
    </v-data-table>

    <ProdutoFormDialog v-model="formOpen" :produto="editing" />

    <CommonConfirmDialog
      v-model="confirmOpen"
      title="Excluir produto"
      :message="`Confirma a exclusão do produto '${toRemove?.nome ?? ''}'?`"
      confirm-text="Excluir"
      :loading="removing"
      @confirm="confirmRemove"
    />
  </v-card>
</template>
