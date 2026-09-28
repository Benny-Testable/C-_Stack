import { useParams } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { getGrid } from '../api/client.js'
import GridCanvas from '../components/GridCanvas.jsx'

export default function GridPage() {
  const { cycleId } = useParams()

  const { data: grid, isLoading, error } = useQuery({
    queryKey: ['grid', cycleId],
    queryFn: () => getGrid(cycleId)
  })

  if (isLoading) return <p>Loading grid...</p>
  if (error) return <p>Failed to load grid.</p>

  return (
    <div>
      <h1>{grid.reviewCycleName}</h1>
      <GridCanvas boxes={grid.boxes} />
    </div>
  )
}
