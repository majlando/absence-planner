import { defineConfig } from '@playwright/test'

// Reads ADMIN_EMAIL and ADMIN_PASSWORD from the project's .env file.
process.loadEnvFile('../.env')

export default defineConfig({
  use: { baseURL: 'http://localhost:8080' },
})
