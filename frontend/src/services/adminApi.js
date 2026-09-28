import axios from 'axios'

const API_BASE = 'http://localhost:5080/api'

export async function loginAdmin(email, password) {
  try {
    const response = await axios.post(`${API_BASE}/admin/login`, { email, password })
    if (!response || !response.data) {
      return { success: false, message: 'Empty admin response' }
    }
    if (response.data.success === false) {
      return { success: false, message: response.data.message || 'Admin login failed' }
    }
    return { success: true, data: response.data }
  } catch (err) {
    return { success: false, message: err?.response?.data?.message || err.message || 'Admin login failed' }
  }
}

export async function loadDashboard(token) {
  try {
    const response = await axios.get(`${API_BASE}/admin/dashboard`, {
      headers: { 'X-Session-Token': token }
    })
    if (!response || !response.data) {
      return { success: false, message: 'Empty dashboard response' }
    }
    return { success: true, data: response.data }
  } catch (err) {
    return { success: false, message: err?.response?.data?.message || err.message || 'Dashboard failed' }
  }
}

export async function loadReports() {
  try {
    const response = await axios.get(`${API_BASE}/reports/all`)
    if (!response || !response.data) {
      return { success: false, message: 'Empty report response' }
    }
    return { success: true, data: response.data }
  } catch (err) {
    return { success: false, message: err?.response?.data?.message || err.message || 'Report failed' }
  }
}
