import { Api } from './ApiClient.ts'

// Same-origin: nginx (Docker) and the Vite dev server proxy /api to the API.
export const api = new Api({ baseUrl: '' })
