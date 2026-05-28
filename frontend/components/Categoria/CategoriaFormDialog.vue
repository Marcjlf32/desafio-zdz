<script setup lang="ts">
import { z } from 'zod'
import type { CategoriaDto } from '~/types/api'

interface Props {
  modelValue: boolean
  categoria?: CategoriaDto | null
}

const props = withDefaults(defineProps<Props>(), {
  categoria: null
})

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  saved: []
}>()

const store = useCategoriasStore()

const schema = z.object({
  nome: z.string().min(5, 'O nome deve ter no mínimo 5 caracteres.'),
  descricao: z.string().max(1000).nullable().optional()
})

const form = reactive({
  nome: '',
  descricao: '' as string | null
})

const errors = reactive<{ nome?: string; descricao?: string }>({})
const saving = ref(false)

const isEdit = computed(() => props.categoria !== null)
const title = computed(() => isEdit.value ? 'Editar categoria' : 'Nova categoria')

const nameIsValid = computed(() => form.nome.trim().length >= 5)

watch(() => props.modelValue, (open) => {
  if (open) {
    form.nome = props.categoria?.nome ?? ''
    form.descricao = props.categoria?.descricao ?? ''
    errors.nome = undefined
    errors.descricao = undefined
  }
})

const close = () => emit('update:modelValue', false)

const persist = () => {
  const payload = { nome: form.nome, descricao: form.descricao }
  if (isEdit.value && props.categoria) {
    return store.update(props.categoria.id, payload)
  }
  return store.create(payload)
}

const save = async () => {
  const result = schema.safeParse(form)
  if (!result.success) {
    const flat = result.error.flatten().fieldErrors
    errors.nome = flat.nome?.[0]
    errors.descricao = flat.descricao?.[0]
    return
  }

  saving.value = true
  try {
    await persist()
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
