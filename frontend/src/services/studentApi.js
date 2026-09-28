import axios from 'axios'

const API_BASE = 'http://localhost:5080/api'

export async function registerStudent(payload) {
  try {
    const response = await axios.post(`${API_BASE}/students/register`, payload)
    if (!response || !response.data) {
      return { success: false, message: 'Empty student response' }
    }
    if (response.data.success === false) {
      return { success: false, message: response.data.message || 'Student registration failed' }
    }
    return { success: true, data: response.data }
  } catch (err) {
    return { success: false, message: err?.response?.data?.message || err.message || 'Student registration failed' }
  }
}

export async function loginStudent(email, password) {
  try {
    const response = await axios.post(`${API_BASE}/students/login`, { email, password })
    if (!response || !response.data) {
      return { success: false, message: 'Empty login response' }
    }
    if (response.data.success === false) {
      return { success: false, message: response.data.message || 'Login failed' }
    }
    return { success: true, data: response.data }
  } catch (err) {
    return { success: false, message: err?.response?.data?.message || err.message || 'Login failed' }
  }
}

export async function updateStudent(id, payload) {
  try {
    const response = await axios.put(`${API_BASE}/students/${id}`, payload)
    if (!response || !response.data) {
      return { success: false, message: 'Empty update response' }
    }
    return { success: true, data: response.data }
  } catch (err) {
    return { success: false, message: err?.response?.data?.message || err.message || 'Update failed' }
  }
}

export async function getStudent(id) {
  try {
    const response = await axios.get(`${API_BASE}/students/${id}`)
    if (!response || !response.data) {
      return { success: false, message: 'Student was not found' }
    }
    return { success: true, data: response.data }
  } catch (err) {
    return { success: false, message: err?.response?.data?.message || err.message || 'Student lookup failed' }
  }
}
