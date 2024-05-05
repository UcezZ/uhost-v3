import api from './api';
import config from '../config.json';

export default {
    /**
     * 
     * @returns {Promise}
     */
    rights: () => api.get(`${config.apiroot}/roles/rights`),

    /**
     * 
     * @param {Number} page 
     * @param {Number} perPage 
     * @param {String} sortBy
     * @param {String} sortDir
     * @returns {Promise}
     */
    getAll: (page, perPage, sortBy, sortDir) => api.get(`${config.apiroot}/roles`, {
        params: {
            page: page > 0 ? page : 1,
            perPage: perPage > 0 ? perPage : 25,
            sortBy: sortBy?.length ? sortBy : 'id',
            sortDirection: sortDir?.length ? sortDir : 'asc'
        }
    }),

    /**
     * 
     * @param {Number} id 
     * @returns {Promise}
     */
    getOne: (id) => api.get(`${config.apiroot}/roles/${id}`),

    /**
     * 
     * @param {String} name 
     * @param {String[]} rights 
     * @returns {Promise}
     */
    create: (name, rights) => api.post(
        `${config.apiroot}/roles`,
        JSON.stringify({ name, rights }),
        {
            headers: {
                'Content-Type': 'application/json'
            }
        }),

    /**
     * 
     * @param {Number} id
     * @param {String} name 
     * @param {String[]} rights 
     * @returns {Promise}
     */
    update: (id, name, rights) => api.put(
        `${config.apiroot}/roles/${id}`,
        JSON.stringify({ name, rights }),
        {
            headers: {
                'Content-Type': 'application/json'
            }
        }),

    /**
     * 
     * @param {Number} id
     * @returns {Promise}
     */
    delete: (id) => api.delete(`${config.apiroot}/roles/${id}`)
};
