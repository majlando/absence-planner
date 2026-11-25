import { Api, type HttpValidationProblemDetails } from './ApiClient.ts'

// Same-origin: nginx (Docker) and the Vite dev server proxy /api to the API.
export const api = new Api({ baseUrl: '' })

// Reads the error messages from a failed API call (a 400 answer).
export async function errorMessages(failed: unknown): Promise<string[]> {
  try {
    const problem = (await (failed as Response).json()) as HttpValidationProblemDetails
    return Object.values(problem.errors ?? {}).flat()
  } catch {
    return ['Something went wrong']
  }
}
