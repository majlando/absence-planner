import { expect, test } from 'vitest'
import { sortRules } from './RulesPage.tsx'

test('the newest rule that has started is the current one', () => {
  const rules = [
    { id: '1', name: 'RaisedSupplementPercent', value: 0.95, startDate: '2020-09-01' },
    { id: '2', name: 'RaisedSupplementPercent', value: 1.48, startDate: '2024-05-01' },
    { id: '3', name: 'RaisedSupplementPercent', value: 2, startDate: '2030-01-01' },
  ]

  const sorted = sortRules(rules, '2026-11-28')

  expect(sorted.map((rule) => [rule.value, rule.current])).toEqual([
    [2, false], // not started yet
    [1.48, true],
    [0.95, false],
  ])
})
