import GridCell from './GridCell.jsx'

const POTENTIAL_LABELS = ['Low Potential', 'Medium Potential', 'High Potential']
const PERFORMANCE_LABELS = ['Low Performance', 'Medium Performance', 'High Performance']

export default function GridCanvas({ boxes }) {
  const cellFor = (perf, pot) => boxes.find((b) => b.performanceLevel === perf && b.potentialLevel === pot)

  return (
    <div className="grid-canvas">
      {[3, 2, 1].map((perf) => (
        <div className="grid-row" key={perf}>
          <span className="grid-row-label">{PERFORMANCE_LABELS[perf - 1]}</span>
          {[1, 2, 3].map((pot) => {
            const box = cellFor(perf, pot)
            return <GridCell key={`${perf}-${pot}`} box={box} />
          })}
        </div>
      ))}
      <div className="grid-col-labels">
        {POTENTIAL_LABELS.map((label) => (
          <span key={label}>{label}</span>
        ))}
      </div>
    </div>
  )
}
