export default function RecordTable({ columns, rows }) {
  return (
    <table>
      <thead>
        <tr>
          {columns.map((column) => (
            <th key={column}>{column}</th>
          ))}
        </tr>
      </thead>
      <tbody>
        {rows.map((row, index) => (
          <tr key={row.id || index}>
            {columns.map((column) => (
              <td key={column}>{String(row[column] ?? '')}</td>
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  )
}
