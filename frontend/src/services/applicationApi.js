import axios from 'axios'

const API_BASE = 'http://localhost:5080/api'

export async function submitApplication(payload) {
  try {
    const response = await axios.post(`${API_BASE}/applications`, payload)
    if (!response || !response.data) {
      return { success: false, message: 'Empty application response' }
    }
    if (response.data.success === false) {
      return { success: false, message: response.data.message || 'Application request failed' }
    }
    return { success: true, data: response.data }
  } catch (err) {
    const body = err?.response?.data
    return { success: false, message: body?.message || err.message || 'Application submit failed' }
  }
}

export async function applicationHistory(studentId) {
  try {
    const response = await axios.get(`${API_BASE}/applications/history/${studentId}`)
    if (!response || !response.data) {
      return { success: false, message: 'Empty history response' }
    }
    return { success: true, data: response.data.items || [] }
  } catch (err) {
    return { success: false, message: err?.response?.data?.message || err.message || 'History lookup failed' }
  }
}
