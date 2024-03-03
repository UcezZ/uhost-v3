import api from './api';
import config from '../config.json';

export default {
    random: (count) => api.get(`${config.apiroot}/videos/random`, {
        params: {
            count: count > 0 ? count : 25
        }
    }),
    search: (query, page, perPage, sortBy, sortDir) => api.get(`${config.apiroot}/videos`, {
        params: {
            name: query,
            page: page > 0 ? page : 1,
            perPage: perPage > 0 ? perPage : 25,
            sortBy: sortBy,
            sortDirection: sortDir
        }
    }),
    getByToken: (token) => api.get(`${config.apiroot}/videos/${token}`, { withCredentials: true, headers: { crossDomain: true } })
};
