import ScholarshipList from '../components/ScholarshipList'

export default function ScholarshipListPage() {
  return (
    <section>
      <div className="panel">
        <h1>Scholarship catalog</h1>
        <p>Filter the award list and open a scholarship to review eligibility rules.</p>
      </div>
      <ScholarshipList />
    </section>
  )
}
