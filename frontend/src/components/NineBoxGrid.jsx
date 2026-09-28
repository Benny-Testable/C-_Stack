import React from 'react';

export const getQuadrantCoordinates = (perfScore, potScore) => {
  const x = perfScore < 3.0 ? 1 : perfScore < 4.0 ? 2 : 3;
  const y = potScore < 3.0 ? 1 : potScore < 4.0 ? 2 : 3;

  if (x === 1 && y === 3) return { block: 1, name: 'Enigma', color: '#F39C12' };
  if (x === 2 && y === 3) return { block: 2, name: 'Growth Potential', color: '#27AE60' };
  if (x === 3 && y === 3) return { block: 3, name: 'Star', color: '#2ECC71' };
  if (x === 1 && y === 2) return { block: 4, name: 'Dilemma', color: '#E67E22' };
  if (x === 2 && y === 2) return { block: 5, name: 'Core Player', color: '#3498DB' };
  if (x === 3 && y === 2) return { block: 6, name: 'High Performer', color: '#1ABC9C' };
  if (x === 1 && y === 1) return { block: 7, name: 'Risk', color: '#E74C3C' };
  if (x === 2 && y === 1) return { block: 8, name: 'Effective', color: '#95A5A6' };
  if (x === 3 && y === 1) return { block: 9, name: 'Solid Professional', color: '#34495E' };
  return { block: 5, name: 'Core Player', color: '#3498DB' };
};

export const getQuadrantColor = (blockNumber) => {
  switch (blockNumber) {
    case 1: return '#F39C12';
    case 2: return '#27AE60';
    case 3: return '#2ECC71';
    case 4: return '#E67E22';
    case 5: return '#3498DB';
    case 6: return '#1ABC9C';
    case 7: return '#E74C3C';
    case 8: return '#95A5A6';
    case 9: return '#34495E';
    default: return '#3498DB';
  }
};

export default function NineBoxGrid({ distribution = [] }) {
  // Visual order: Top row (y=3: 1, 2, 3), Mid row (y=2: 4, 5, 6), Bottom row (y=1: 7, 8, 9)
  const orderedBlocks = [1, 2, 3, 4, 5, 6, 7, 8, 9];

  return (
    <div className="grid-board">
      {orderedBlocks.map(blockId => {
        const data = distribution.find(d => d.blockNumber === blockId) || { count: 0, employees: [] };
        const color = getQuadrantColor(blockId);

        return (
          <div key={blockId} className="quadrant-card" style={{ borderTopColor: color }}>
            <div className="quadrant-title">
              <span>Block {blockId}</span>
              <span className="badge" style={{ backgroundColor: color }}>
                {data.count} Assigned
              </span>
            </div>
            <div className="employees-list">
              {data.employees.map(emp => (
                <div key={emp.employeeId} className="employee-chip">
                  <strong>{emp.employeeName}</strong> ({emp.department})
                  <div>Perf: {emp.performanceScore} | Pot: {emp.potentialScore}</div>
                </div>
              ))}
            </div>
          </div>
        );
      })}
    </div>
  );
}
