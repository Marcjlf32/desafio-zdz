<script setup lang="ts">
const categorias = useCategoriasStore()
const produtos = useProdutosStore()

const visible = ref(false)
const message = ref('')

const showError = (detail: string | null) => {
  if (!detail) return
  message.value = detail
  visible.value = true
}

categorias.$subscribe((_mutation, state) => {
  if (state.error) showError(state.error)
})

produtos.$subscribe((_mutation, state) => {
  if (state.error) showError(state.error)
})

const handleClose = () => {
  visible.value = false
  categorias.clearError()
  produtos.clearError()
}
</script>

<template>
  <v-snackbar
    v-model="visible"
    color="error"
    location="top right"
    :timeout="6000"
  >
    {{ message }}
    <template #actions>
      <v-btn variant="text" @click="handleClose">Fechar</v-btn>
    </template>
  </v-snackbar>
</template>
