import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { getReviewCycles } from '../api/client.js'

export default function DashboardPage() {
  const { data: cycles, isLoading, error } = useQuery({
    queryKey: ['reviewCycles'],
    queryFn: getReviewCycles
  })

  if (isLoading) return <p>Loading review cycles...</p>
  if (error) return <p>Failed to load review cycles.</p>

  return (
    <div>
      <h1>9-Block Talent Matrix</h1>
      <h2>Review Cycles</h2>
      <ul>
        {cycles?.map((cycle) => (
          <li key={cycle.id}>
            <Link to={`/grid/${cycle.id}`}>{cycle.name}</Link> — {cycle.status}
          </li>
        ))}
      </ul>
    </div>
  )
}
