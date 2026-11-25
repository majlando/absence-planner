import { expect, test } from 'vitest'
import { errorMessages } from './index.ts'

test('reads the messages from a 400 answer', async () => {
  const failed = Response.json({ errors: { Email: ['Bad email'], Password: ['Too short'] } }, { status: 400 })

  expect(await errorMessages(failed)).toEqual(['Bad email', 'Too short'])
})

test('gives a general message when the answer has no messages', async () => {
  expect(await errorMessages(new Response('oops', { status: 500 }))).toEqual(['Something went wrong'])
})
