const API_BASE_URL = '/api';

export async function fetchQuadrants() {
  const response = await fetch(`${API_BASE_URL}/nineboxgrid/quadrants`);
  if (!response.ok) throw new Error('Failed to fetch 9-box quadrants');
  return response.json();
}

export async function fetchDistribution(cycleId) {
  const url = cycleId 
    ? `${API_BASE_URL}/nineboxgrid/distribution?cycleId=${cycleId}`
    : `${API_BASE_URL}/nineboxgrid/distribution`;
  const response = await fetch(url);
  if (!response.ok) throw new Error('Failed to fetch matrix distribution');
  return response.json();
}

export async function fetchPlatformInfo() {
  const response = await fetch(`${API_BASE_URL}/platforminfo`);
  if (!response.ok) throw new Error('Failed to fetch platform build info');
  return response.json();
}

export async function calculateQuadrant(performanceScore, potentialScore) {
  const response = await fetch(`${API_BASE_URL}/nineboxgrid/calculate`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ performanceScore, potentialScore })
  });
  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.error || 'Failed to calculate quadrant');
  }
  return response.json();
}
