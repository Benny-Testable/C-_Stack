import React from 'react';
import { getQuadrantColor } from '../utils/quadrantUtils';

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
