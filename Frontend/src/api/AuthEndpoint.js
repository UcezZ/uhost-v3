import api from './api';
import config from '../config.json';

export default {
    info: () => api.get(`${config.apiroot}/auth/info`),
    check: () => api.get(`${config.apiroot}/auth/check`),
    logout: () => api.post(`${config.apiroot}/auth/logout`),

    /**
     * 
     * @param {String} login 
     * @param {String} password 
     * @returns 
     */
    login: (login, password) => api.postForm(`${config.apiroot}/auth/login`, {
        login: login ?? '',
        password: password ?? ''
    })
};
