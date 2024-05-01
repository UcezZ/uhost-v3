import api from './api';
import config from '../config.json';

export default {
    /**
     * 
     * @param {Number} page 
     * @param {Number} perPage 
     * @param {String} sortBy
     * @param {String} sortDir
     * @param {Number} userId 
     * @returns 
     */
    get: (page, perPage, sortBy, sortDir, userId) => api.get(`${config.apiroot}/sessions`, {
        params: {
            page: page > 0 ? page : 1,
            perPage: perPage > 0 ? perPage : 50,
            sortBy: sortBy?.length ? sortBy : 'expiresIn',
            sortDirection: sortDir?.length ? sortDir : 'desc',
            userId
        }
    }),

    /**
     * 
     * @param {String} guid 
     * @returns 
     */
    terminate: (guid) => api.delete(`${config.apiroot}/sessions/${guid}`)
};
