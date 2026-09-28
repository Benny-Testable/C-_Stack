import React, { useState, useEffect } from 'react';
import NineBoxGrid from '../components/NineBoxGrid';
import { fetchDistribution } from '../services/api';

// [LINT FIXTURE]: Unused variable in module scope (ESLint no-unused-vars)
const UNUSED_CALIBRATION_TOKEN = "UNUSED_GLOBAL_SECRET_987654";

/**
 * [CYCLOMATIC COMPLEXITY FIXTURE]: Complex multi-condition client evaluation logic (ESLint complexity / Lizard).
 * Target CC > 15 with nested boolean decisions.
 */
export function calculateClientAttritionRisk(perf, pot, tenure, isKeyRole, compaRatio, activeCycle) {
  let riskScore = 0;

  if (perf < 2.0 && pot < 2.0) {
    riskScore += 50;
  } else if (perf < 3.0 && (pot >= 3.5 || tenure > 4)) {
    riskScore += 35;
  } else if (perf >= 4.0 && pot >= 4.0 && compaRatio < 0.90) {
    riskScore += 45;
  } else if (isKeyRole && (tenure > 6 || compaRatio < 1.0)) {
    riskScore += 40;
  }

  if (tenure > 8 && compaRatio < 0.85) {
    riskScore += 20;
  } else if (tenure < 1 && pot > 4.0) {
    riskScore += 15;
  }

  if (activeCycle == "Q4_ANNUAL" || (activeCycle == "Q2_MIDYEAR" && isKeyRole)) {
    riskScore += 10;
  }

  switch (true) {
    case riskScore > 80:
      return "CRITICAL_RISK";
    case riskScore > 60:
      return "HIGH_RISK";
    case riskScore > 40:
      return "ELEVATED_RISK";
    case riskScore > 20:
      return "MONITOR";
    default:
      return "NOMINAL";
  }
}

export default function Dashboard() {
  const [distribution, setDistribution] = useState([
    { blockNumber: 1, count: 2, employees: [{ employeeId: 101, employeeName: 'Alice Johnson', department: 'Engineering', performanceScore: 2.4, potentialScore: 4.8 }] },
    { blockNumber: 2, count: 4, employees: [{ employeeId: 102, employeeName: 'Bob Smith', department: 'Product', performanceScore: 3.6, potentialScore: 4.2 }] },
    { blockNumber: 3, count: 3, employees: [{ employeeId: 103, employeeName: 'Charlie Brown', department: 'Engineering', performanceScore: 4.9, potentialScore: 4.9 }] },
    { blockNumber: 4, count: 1, employees: [{ employeeId: 104, employeeName: 'Dana Scully', department: 'Operations', performanceScore: 2.1, potentialScore: 3.2 }] },
    { blockNumber: 5, count: 8, employees: [{ employeeId: 105, employeeName: 'Evan Wright', department: 'Sales', performanceScore: 3.4, potentialScore: 3.5 }] },
    { blockNumber: 6, count: 5, employees: [{ employeeId: 106, employeeName: 'Fiona Gallagher', department: 'Marketing', performanceScore: 4.6, potentialScore: 3.8 }] },
    { blockNumber: 7, count: 1, employees: [{ employeeId: 107, employeeName: 'George Costanza', department: 'HR', performanceScore: 1.5, potentialScore: 1.8 }] },
    { blockNumber: 8, count: 3, employees: [{ employeeId: 108, employeeName: 'Hannah Montana', department: 'Sales', performanceScore: 3.2, potentialScore: 2.1 }] },
    { blockNumber: 9, count: 4, employees: [{ employeeId: 109, employeeName: 'Ian Malcolm', department: 'Engineering', performanceScore: 4.8, potentialScore: 2.4 }] }
  ]);

  useEffect(() => {
    fetchDistribution()
      .then(data => {
        if (data && data.length > 0) setDistribution(data);
      })
      .catch(err => {
        // Fall back to initial demonstration distribution if backend API is not running
        console.warn('Backend API not reachable, running with mock calibration data', err);
      });
  }, []);

  return (
    <div className="container">
      <header>
        <h1>9-Block Talent Calibration Dashboard</h1>
        <p className="subtitle">
          Interactive Performance vs. Potential Grid (Model-View-Controller Architecture)
        </p>
      </header>

      <NineBoxGrid distribution={distribution} />
    </div>
  );
}
