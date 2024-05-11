import api from './api';
import config from '../config.json';

export default {
    /**
     * @param {String} token
     * @returns {Promise}
     */
    get: (token) => api.get(`${config.apiroot}/reactions/${token}`),

    /**
     * 
     * @param {String} token 
     * @param {String} value 
     * @returns {Promise}
     */
    post: (token, value) => api.post(`${config.apiroot}/reactions/${token}/${value}`),

    /**
     * 
     * @param {String} token 
     * @returns {Promise}
     */
    delete: (token) => api.delete(`${config.apiroot}/reactions/${token}`)
}