import ApplicationList from '../components/ApplicationList'

export default function ApplicationListPage() {
  return (
    <section>
      <div className="panel">
        <h1>Application queue</h1>
        <p>Track status, then approve or reject a submitted application.</p>
      </div>
      <ApplicationList />
    </section>
  )
}
