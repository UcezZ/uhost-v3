import api from './api';
import config from '../config.json';

export default {
    /**
     * 
     * @param {String} videoToken 
     * @param {Number} page 
     * @param {Number} perPage 
     * @returns 
     */
    get: (videoToken, page, perPage) => api.get(`${config.apiroot}/comments/${videoToken}`, {
        params: {
            page: page > 0 ? page : 1,
            perPage: perPage > 1 ? perPage : 25,
            sortBy: 'CreatedAt',
            sortDirection: 'Desc'
        }
    }),

    /**
     * 
     * @param {String} videoToken 
     * @param {String} text 
     * @returns 
     */
    post: (videoToken, text) => api.postForm(`${config.apiroot}/comments/${videoToken}`, {
        text: text?.length > 0 ? text : ''
    }),

    /**
     * 
     * @param {String} videoToken 
     * @param {Number} id 
     * @returns 
     */
    delete: (videoToken, id) => api.delete(`${config.apiroot}/comments/${videoToken}/${id}`)
};
