<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { z } from 'zod'
import type { ProdutoDto } from '~/types/api'

interface Props {
  modelValue: boolean
  produto?: ProdutoDto | null
}

const props = withDefaults(defineProps<Props>(), {
  produto: null
})

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  saved: []
}>()

const produtosStore = useProdutosStore()
const categoriasStore = useCategoriasStore()
const { items: categorias } = storeToRefs(categoriasStore)

const schema = z.object({
  nome: z.string().min(5, 'O nome deve ter no mínimo 5 caracteres.'),
  descricao: z.string().max(1000).nullable().optional(),
  preco: z.number({ invalid_type_error: 'Informe o preço.' }).min(0, 'O preço deve ser maior ou igual a zero.'),
  categoriaId: z.number({ invalid_type_error: 'Selecione uma categoria.' }).int().positive('Selecione uma categoria.')
})

const form = reactive({
  nome: '',
  descricao: '' as string | null,
  preco: null as number | null,
  categoriaId: null as number | null
})

const errors = reactive<Record<string, string | undefined>>({})
const saving = ref(false)

const isEdit = computed(() => props.produto !== null)
const title = computed(() => isEdit.value ? 'Editar produto' : 'Novo produto')
const nameIsValid = computed(() => form.nome.trim().length >= 5)

watch(() => props.modelValue, async (open) => {
  if (open) {
    if (!categorias.value.length) {
      await categoriasStore.fetchAll()
    }
    form.nome = props.produto?.nome ?? ''
    form.descricao = props.produto?.descricao ?? ''
    form.preco = props.produto?.preco ?? null
    form.categoriaId = props.produto?.categoriaId ?? null
    Object.keys(errors).forEach(k => { errors[k] = undefined })
  }
})

const close = () => emit('update:modelValue', false)

const persist = (payload: {
  nome: string
  descricao: string | null | undefined
  preco: number
  categoriaId: number
}) => {
  if (isEdit.value && props.produto) {
    return produtosStore.update(props.produto.id, payload)
  }
  return produtosStore.create(payload)
}

const save = async () => {
  const result = schema.safeParse(form)
  if (!result.success) {
    const flat = result.error.flatten().fieldErrors
    errors.nome = flat.nome?.[0]
    errors.descricao = flat.descricao?.[0]
    errors.preco = flat.preco?.[0]
    errors.categoriaId = flat.categoriaId?.[0]
    return
  }

  saving.value = true
  try {
    await persist(result.data)
    emit('saved')
    close()
  } catch {
    close()
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <v-dialog :model-value="modelValue" @update:model-value="emit('update:modelValue', $event)" max-width="560" persistent>
    <v-card>
      <v-card-title class="text-h6">{{ title }}</v-card-title>
      <v-card-text>
        <v-form @submit.prevent="save">
          <v-text-field
            v-model="form.nome"
            label="Nome"
            :error-messages="errors.nome"
            :counter="200"
            autofocus
            required
          />
          <v-textarea
            v-model="form.descricao"
            label="Descrição"
            :error-messages="errors.descricao"
            :counter="1000"
            rows="3"
          />
          <v-text-field
            v-model.number="form.preco"
            type="number"
            label="Preço"
            prefix="R$"
            :error-messages="errors.preco"
            min="0"
            step="0.01"
          />
          <v-select
            v-model="form.categoriaId"
            label="Categoria"
            :items="categorias"
            item-title="nome"
            item-value="id"
            :error-messages="errors.categoriaId"
            required
          />
        </v-form>
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn variant="text" :disabled="saving" @click="close">Cancelar</v-btn>
        <v-btn
          color="primary"
          variant="elevated"
          :disabled="!nameIsValid || saving"
          :loading="saving"
          @click="save"
        >
          Salvar
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
