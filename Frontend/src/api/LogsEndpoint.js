import api from './api';
import config from '../config.json';

export default {
    /**
     * 
     * @returns {Promise}
     */
    events: () => api.get(`${config.apiroot}/logs/events`),

    /**
     * 
     * @param {Number} page 
     * @param {Number} perPage 
     * @param {String} sortBy
     * @param {String} sortDir
     * @param {String[]} events 
     * @param {Number} userId 
     * @param {String} dateFrom 
     * @param {String} dateTo 
     * @returns 
     */
    get: function (page, perPage, sortBy, sortDir, events, userId, dateFrom, dateTo) {
        var url = `${config.apiroot}/logs`;

        if (events?.length > 0) {
            url += `?${events.map(e => `events=${e}`).join('&')}`;
        }

        return api.get(url, {
            params: {
                page: page > 0 ? page : 1,
                perPage: perPage > 0 ? perPage : 50,
                sortBy: sortBy?.length ? sortBy : 'createdAt',
                sortDirection: sortDir?.length ? sortDir : 'desc',
                userId,
                dateFrom,
                dateTo,
            }
        })
    }
};
