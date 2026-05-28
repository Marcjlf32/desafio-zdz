<script setup lang="ts">
import { storeToRefs } from 'pinia'
import type { CategoriaDto } from '~/types/api'

const store = useCategoriasStore()
const { items, loading } = storeToRefs(store)

const formOpen = ref(false)
const confirmOpen = ref(false)
const editing = ref<CategoriaDto | null>(null)
const toRemove = ref<CategoriaDto | null>(null)
const removing = ref(false)

onMounted(() => store.fetchAll())

const openCreate = () => {
  editing.value = null
  formOpen.value = true
}

const openEdit = (categoria: CategoriaDto) => {
  editing.value = categoria
  formOpen.value = true
}

const askRemove = (categoria: CategoriaDto) => {
  toRemove.value = categoria
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

const headers = [
  { title: 'ID', key: 'id', width: 80 },
  { title: 'Nome', key: 'nome' },
  { title: 'Descrição', key: 'descricao' },
  { title: 'Ações', key: 'actions', sortable: false, width: 180 }
]
</script>

<template>
  <v-card>
    <v-card-title class="d-flex align-center">
      <span>Categorias</span>
      <v-spacer />
      <v-btn color="primary" prepend-icon="mdi-plus" @click="openCreate">Nova categoria</v-btn>
    </v-card-title>

    <v-data-table
      :headers="headers"
      :items="items"
      :loading="loading"
      density="comfortable"
      no-data-text="Nenhuma categoria cadastrada."
    >
      <template #item.actions="{ item }">
        <v-btn icon="mdi-pencil" variant="text" size="small" color="primary" @click="openEdit(item)" />
        <v-btn icon="mdi-delete" variant="text" size="small" color="error" @click="askRemove(item)" />
      </template>
    </v-data-table>

    <CategoriaFormDialog v-model="formOpen" :categoria="editing" />

    <CommonConfirmDialog
      v-model="confirmOpen"
      title="Excluir categoria"
      :message="`Confirma a exclusão da categoria '${toRemove?.nome ?? ''}'?`"
      confirm-text="Excluir"
      :loading="removing"
      @confirm="confirmRemove"
    />
  </v-card>
</template>
