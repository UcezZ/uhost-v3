import api from './api';
import config from '../config.json';

export default {
    info: () => api.get(`${config.apiroot}/auth/info`),
    check: () => api.get(`${config.apiroot}/auth/check`),
    logout: () => api.post(`${config.apiroot}/auth/logout`),
    login: (formData) => api.post(`${config.apiroot}/auth/login`, formData)
};
