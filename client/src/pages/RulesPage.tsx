import { redirect, useLoaderData } from 'react-router'
import { api } from '../api/index.ts'
import type { RuleResponse } from '../api/ApiClient.ts'

// Only admins may see this page. Others are sent to the home page.
export async function rulesLoader() {
  try {
    const response = await api.api.getApiRules()
    return response.data
  } catch {
    return redirect('/')
  }
}

// Sorts the rules by name, newest start date first.
// For each name, the first row that has started (start date is today or before) is the current value.
export function sortRules(rules: RuleResponse[], today: string) {
  const sorted = [...rules].sort((a, b) => a.name.localeCompare(b.name) || b.startDate.localeCompare(a.startDate))
  const found: string[] = []
  return sorted.map((rule) => {
    const current = rule.startDate <= today && !found.includes(rule.name)
    if (current) found.push(rule.name)
    return { ...rule, current }
  })
}

export function RulesPage() {
  const rules = useLoaderData() as RuleResponse[]
  const today = new Date().toISOString().slice(0, 10)

  return (
    <>
      <h1>Rules</h1>
      <p>A changed rule gets a new row with a new start date. Old rows are kept as history.</p>
      <table>
        <thead>
          <tr>
            <th>Rule</th>
            <th>Value</th>
            <th>Start date</th>
            <th>Current</th>
          </tr>
        </thead>
        <tbody>
          {sortRules(rules, today).map((rule) => (
            <tr key={rule.id}>
              <td>{rule.name}</td>
              <td>{rule.value}</td>
              <td>{rule.startDate}</td>
              <td>{rule.current ? 'Yes' : ''}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  )
}
