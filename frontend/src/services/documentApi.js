import axios from 'axios'

const API_BASE = 'http://localhost:5080/api'

export async function uploadDocument(payload) {
  try {
    const response = await axios.post(`${API_BASE}/documents`, payload)
    if (!response || !response.data) {
      return { success: false, message: 'Empty document response' }
    }
    if (response.data.success === false) {
      return { success: false, message: response.data.message || 'Document request failed' }
    }
    return { success: true, data: response.data }
  } catch (err) {
    return { success: false, message: err?.response?.data?.message || err.message || 'Document upload failed' }
  }
}
