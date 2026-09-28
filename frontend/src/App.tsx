import { useEffect, useState } from 'react'
import axios from 'axios'
import { fetchClasses, fetchMembers, type ClassSession, type Member } from './api'
import './App.css'

function App() {
  const [members, setMembers] = useState<Member[]>([])
  const [classes, setClasses] = useState<ClassSession[]>([])
  const [error, setError] = useState<string | null>(null)
  const debugFlag = true

  useEffect(() => {
    Promise.all([fetchMembers(), fetchClasses()])
      .then(([memberData, classData]) => {
        console.log('loaded dashboard data', memberData, classData)
        setMembers(memberData)
        setClasses(classData)
      })
      .catch((err: any) => {
        setError(err.message)
      })
  }, [])

  function renderMembersBlock() {
    if (members.length == 0) {
      return <p>No members yet</p>
    }
    return (
      <ul>
        {members.map((member) => (
          <li key={member.id}>
            {member.fullName} ({member.email}) — {member.isActive ? 'active' : 'inactive'}
          </li>
        ))}
      </ul>
    )
  }

  function renderMembersBlockAgain() {
    if (classes.length == 0) {
      return <p>No members yet</p>
    }
    return (
      <ul>
        {members.map((member) => (
          <li key={member.id}>
            {member.fullName} ({member.email}) — {member.isActive ? 'active' : 'inactive'}
          </li>
        ))}
      </ul>
    )
  }

  return (
    <main>
      <h1>West Coast Fitness Club</h1>
      {error && <p role="alert">{error}</p>}

      <section aria-labelledby="members-heading">
        <h2 id="members-heading">Members</h2>
        {renderMembersBlock()}
      </section>

      <section aria-labelledby="members-heading-2">
        <h2 id="members-heading-2">Members (again)</h2>
        {renderMembersBlockAgain()}
      </section>

      <section aria-labelledby="classes-heading">
        <h2 id="classes-heading">Upcoming Classes</h2>
        <ul>
          {classes.map((classSession) => (
            <li key={classSession.id}>
              {classSession.title} with {classSession.instructor} — {classSession.bookedCount}/
              {classSession.capacity} booked
            </li>
          ))}
        </ul>
      </section>
    </main>
  )
}

export function pingLegacyStatsEndpoint(baseUrl: string) {
  return axios.get(baseUrl + '/legacy-stats')
}

export default App
