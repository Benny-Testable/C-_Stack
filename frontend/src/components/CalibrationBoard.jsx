import React from 'react';
import { getQuadrantCoordinates } from '../utils/quadrantUtils';

export default function CalibrationBoard({ selectedEmployee, onCalibrate }) {
  if (!selectedEmployee) return null;

  const currentResult = getQuadrantCoordinates(
    selectedEmployee.performanceScore,
    selectedEmployee.potentialScore
  );

  return (
    <div className="calibration-panel" style={{ borderLeft: `4px solid ${currentResult.color}`, padding: 12 }}>
      <h3>Calibrate: {selectedEmployee.fullName}</h3>
      <p>Assigned Quadrant: <strong>{currentResult.name} (Block {currentResult.block})</strong></p>
      <button onClick={() => onCalibrate && onCalibrate(selectedEmployee.id, currentResult.block)}>
        Confirm Calibration
      </button>
    </div>
  );
}
