export default function StatusBadge({ value }) {
  return <span className="badge">{value || 'Unknown'}</span>
}
