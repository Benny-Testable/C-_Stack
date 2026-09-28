import React from 'react';

export default function BuildInfoBanner({ info }) {
  if (!info) return null;

  return (
    <div className="build-info-banner">
      <span><strong>Branch:</strong> {info.branch}</span>
      <span><strong>Build Tool:</strong> {info.buildTool}</span>
      <span><strong>Architecture:</strong> {info.architecture}</span>
    </div>
  );
}
