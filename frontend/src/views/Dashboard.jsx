import React, { useState, useEffect } from 'react';
import NineBoxGrid from '../components/NineBoxGrid';
import { fetchDistribution } from '../services/api';

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
