export const useCrudApi = <TDto, TCreate, TUpdate>(path: string) => {
  const config = useRuntimeConfig()
  const baseUrl = `${config.public.apiBaseUrl}${path}`

  const list = () => $fetch<TDto[]>(baseUrl)
  const create = (dto: TCreate) => $fetch<TDto>(baseUrl, { method: 'POST', body: dto })
  const update = (id: number, dto: TUpdate) =>
    $fetch<void>(`${baseUrl}/${id}`, { method: 'PUT', body: dto })
  const remove = (id: number) =>
    $fetch<void>(`${baseUrl}/${id}`, { method: 'DELETE' })

  return { list, create, update, remove }
}
