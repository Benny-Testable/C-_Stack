import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { getReviewCycles, apiClient } from '../api/client.js'

export default function CycleAdminPage() {
  const queryClient = useQueryClient()
  const [name, setName] = useState('')
  const [startDate, setStartDate] = useState('')
  const [endDate, setEndDate] = useState('')

  const { data: cycles } = useQuery({ queryKey: ['reviewCycles'], queryFn: getReviewCycles })

  const createCycle = useMutation({
    mutationFn: (payload) => apiClient.post('/reviewcycles', payload),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['reviewCycles'] })
  })

  const handleSubmit = (e) => {
    e.preventDefault()
    createCycle.mutate({ name, startDate, endDate, status: 0 })
    setName('')
    setStartDate('')
    setEndDate('')
  }

  return (
    <div>
      <h1>Manage Review Cycles</h1>
      <form onSubmit={handleSubmit}>
        <input value={name} onChange={(e) => setName(e.target.value)} placeholder="Cycle name" required />
        <input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} required />
        <input type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} required />
        <button type="submit">Create Cycle</button>
      </form>
      <ul>
        {cycles?.map((c) => (
          <li key={c.id}>{c.name}</li>
        ))}
      </ul>
    </div>
  )
}
