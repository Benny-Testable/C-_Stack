import EmployeeCard from './EmployeeCard.jsx'

export default function GridCell({ box }) {
  if (!box) return <div className="grid-cell" />

  return (
    <div className="grid-cell">
      <div className="grid-cell-title">{box.position}</div>
      {box.employees.map((emp) => (
        <EmployeeCard key={emp.assessmentId} employee={emp} />
      ))}
    </div>
  )
}
