import axios from 'axios'

const API_BASE = 'http://localhost:5080/api'

export async function getScholarship(id) {
  try {
    const response = await axios.get(`${API_BASE}/scholarships/${id}`)
    if (!response || !response.data) {
      return { success: false, message: 'Empty scholarship response' }
    }
    if (response.data.success === false) {
      return { success: false, message: response.data.message || 'Scholarship request failed' }
    }
    return { success: true, data: response.data }
  } catch (err) {
    return { success: false, message: err?.response?.data?.message || err.message || 'Scholarship lookup failed' }
  }
}

export async function createScholarship(payload) {
  try {
    const response = await axios.post(`${API_BASE}/scholarships`, payload)
    if (!response || !response.data) {
      return { success: false, message: 'Empty scholarship response' }
    }
    return { success: true, data: response.data }
  } catch (err) {
    return { success: false, message: err?.response?.data?.message || err.message || 'Scholarship create failed' }
  }
}
