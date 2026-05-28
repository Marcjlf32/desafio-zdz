import type { ProblemDetails } from '~/types/api'

export function extractErrorDetail(err: unknown): string {
  const fetchErr = err as { data?: ProblemDetails; message?: string }
  return fetchErr?.data?.detail ?? fetchErr?.message ?? 'Erro inesperado.'
}
