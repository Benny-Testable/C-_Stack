import axios from 'axios'

export const apiClient = axios.create({
  baseURL: '/api'
})

export const getEmployees = (departmentId) =>
  apiClient.get('/employees', { params: { departmentId } }).then((res) => res.data)

export const getDepartments = () => apiClient.get('/departments').then((res) => res.data)

export const getReviewCycles = () => apiClient.get('/reviewcycles').then((res) => res.data)

export const getGrid = (reviewCycleId, params) =>
  apiClient.get(`/grid/${reviewCycleId}`, { params }).then((res) => res.data)

export const createAssessment = (payload) =>
  apiClient.post('/assessments', payload).then((res) => res.data)

export const updateAssessment = (id, payload) =>
  apiClient.put(`/assessments/${id}`, payload).then((res) => res.data)
