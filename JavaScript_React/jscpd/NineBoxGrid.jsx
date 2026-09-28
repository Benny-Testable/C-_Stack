import React, { useState } from 'react';

export const NineBoxGrid = ({ employees = [] }) => {
  const [selectedQuadrant, setSelectedQuadrant] = useState(null);

  const getQuadrantInfo = (perfScore, potScore) => {
    if (perfScore < 1.0 || perfScore > 5.0 || potScore < 1.0 || potScore > 5.0) {
      return { quadrant: 'Invalid', tier: 'Invalid', color: '#B0BEC5' };
    }

    const perf = perfScore < 3.0 ? 1 : perfScore < 4.0 ? 2 : 3;
    const pot = potScore < 3.0 ? 1 : potScore < 4.0 ? 2 : 3;
    const key = `${perf}-${pot}`;

    const mapping = {
      '3-3': { quadrant: 'Star', tier: 'High Value', color: '#2ECC71' },
      '2-3': { quadrant: 'Growth Potential', tier: 'High Value', color: '#27AE60' },
      '1-3': { quadrant: 'Enigma', tier: 'Development', color: '#F39C12' },
      '3-2': { quadrant: 'High Performer', tier: 'High Value', color: '#3498DB' },
      '2-2': { quadrant: 'Core Player', tier: 'Development', color: '#F1C40F' },
      '1-2': { quadrant: 'Dilemma', tier: 'Intervention', color: '#E67E22' },
      '3-1': { quadrant: 'Solid Professional', tier: 'Specialist', color: '#9B59B6' },
      '2-1': { quadrant: 'Effective', tier: 'Specialist', color: '#1ABC9C' },
      '1-1': { quadrant: 'Risk', tier: 'Critical Risk', color: '#E74C3C' }
    };

    return mapping[key] || { quadrant: 'Unassigned', tier: 'Unknown', color: '#95A5A6' };
  };

  const getActionBadgeClass = (tier) => {
    switch (tier) {
      case 'High Value':
        return 'badge-success';
      case 'Development':
        return 'badge-warning';
      case 'Specialist':
        return 'badge-info';
      case 'Critical Risk':
      case 'Intervention':
        return 'badge-danger';
      default:
        return 'badge-secondary';
    }
  };

  return (
    <div className="nine-box-container">
      <h2>9-Box Talent Calibration Grid</h2>
      <div className="grid-layout">
        {employees.map((emp) => {
          const info = getQuadrantInfo(emp.performance, emp.potential);
          return (
            <div key={emp.id} className="employee-card" style={{ borderColor: info.color }}>
              <h4>{emp.name}</h4>
              <span className={`badge ${getActionBadgeClass(info.tier)}`}>{info.quadrant}</span>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default NineBoxGrid;
