export default function EmployeeCard({ employee }) {
  return (
    <div className="employee-card">
      <strong>{employee.name}</strong>
      <div>{employee.jobTitle}</div>
      <div className="employee-department">{employee.department}</div>
    </div>
  )
}
