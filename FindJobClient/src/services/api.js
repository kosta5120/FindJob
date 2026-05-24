import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
  headers: { 'Content-Type': 'application/json' },
});

// Attach JWT token for admin requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('adminToken');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

// Jobs
export const getJobs = (params) => api.get('/jobs', { params });
export const createJob = (data) => api.post('/jobs', data);
export const deleteJob = (id) => api.delete(`/jobs/${id}`);

// Categories & Subcategories
export const getCategories = () => api.get('/categories');
export const getSubcategories = (categoryId) => api.get(`/categories/${categoryId}/subcategories`);

// Regions & Cities
export const getRegions = () => api.get('/regions');
export const getCities = (regionId) => api.get(`/regions/${regionId}/cities`);

// Job Types
export const getJobTypes = () => api.get('/job-types');

// Application (send CV)
export const applyToJob = (data) => api.post('/apply', data, {
  headers: { 'Content-Type': 'multipart/form-data' },
});

// Reports
export const reportJob = (postId) => api.post('/reports', { postId });
export const getReports = () => api.get('/admin/reports');
export const deleteReport = (id) => api.delete(`/admin/reports/${id}`);
export const deleteJobAndReport = (id) => api.delete(`/admin/reports/${id}/with-post`);

// Stats
export const getMonthlyStats = () => api.get('/admin/stats/monthly');
export const getCityStats = () => api.get('/admin/stats/cities');

// Auth
export const adminLogin = (credentials) => api.post('/auth/login', credentials);

export default api;
