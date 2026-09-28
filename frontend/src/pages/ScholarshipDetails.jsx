import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { getScholarship } from '../services/scholarshipApi'

export default function ScholarshipDetails() {
  const { id } = useParams()
  const [item, setItem] = useState(null)
  const [error, setError] = useState('')

  useEffect(() => {
    getScholarship(id).then((result) => {
      if (!result.success) {
        setError(result.message)
        return
      }
      setItem(result.data)
    })
  }, [id])

  if (error) {
    return <p className="error">{error}</p>
  }
  if (!item) {
    return <p>Loading scholarship...</p>
  }

  return (
    <section className="panel">
      <h1>{item.name}</h1>
      <p>{item.sponsor} · Award {item.awardAmount} · Minimum GPA {item.minimumGpa}</p>
      <p>Credits {item.minimumCreditHours} · Income ceiling {item.maximumIncome} · Seats {item.seats}</p>
      <p>Major {item.requiredMajor} · Residency {item.requiredResidency}</p>
      {/* INTENTIONAL NEGATIVE TEST DATA: description is rendered as HTML. */}
      <article dangerouslySetInnerHTML={{ __html: item.description || '' }} />
    </section>
  )
}
