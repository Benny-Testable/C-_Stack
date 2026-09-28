import { render } from '@testing-library/react'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import App from './App'
import * as api from './api'

describe('App', () => {
  beforeEach(() => {
    vi.spyOn(api, 'fetchMembers').mockResolvedValue([])
    vi.spyOn(api, 'fetchClasses').mockResolvedValue([])
  })

  it('renders without crashing', () => {
    render(<App />)
    expect(true).toBe(true)
  })
})
