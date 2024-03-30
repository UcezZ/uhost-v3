import api from './api';
import config from '../config.json';

export default {
    /**
     * 
     * @param {Number} count 
     * @returns {Promise}
     */
    random: (count) => api.get(`${config.apiroot}/videos/random`, {
        params: {
            count: count > 0 ? count : 25
        }
    }),

    /**
     * 
     * @param {String} query 
     * @param {Number} page 
     * @param {Number} perPage 
     * @param {String} sortBy 
     * @param {String} sortDir 
     * @returns {Promise}
     */
    search: (query, page, perPage, sortBy, sortDir) => api.get(`${config.apiroot}/videos`, {
        params: {
            name: query,
            page: page > 0 ? page : 1,
            perPage: perPage > 0 ? perPage : 25,
            sortBy: sortBy,
            sortDirection: sortDir
        }
    }),

    /**
     * 
     * @param {String} token 
     * @returns {Promise}
     */
    getByToken: (token) => api.get(`${config.apiroot}/videos/${token}`, {
        withCredentials: true,
        headers: {
            crossDomain: true
        }
    }),

    /**
     * 
     * @param {String} token 
     * @param {String} name 
     * @param {String} desc 
     * @param {Boolean} isPrivate 
     * @param {Boolean} isHidden 
     * @param {Boolean} allowComments 
     * @param {Boolean} allowReactions 
     * @returns {Promise}
     */
    edit: (token, name, desc, isPrivate, isHidden, allowComments, allowReactions) => api.putForm(`${config.apiroot}/videos/${token}`, {
        name: name ?? '',
        description: desc ?? '',
        isPrivate: isPrivate ?? false,
        isHidden: isHidden ?? false,
        allowComments: allowComments ?? false,
        allowReactions: allowReactions ?? false
    }),

    /**
     * 
     * @param {String} file 
     * @param {String} name 
     * @param {String} desc 
     * @param {Boolean} isPrivate 
     * @param {Boolean} isHidden 
     * @param {Boolean} allowComments 
     * @param {Boolean} allowReactions 
     * @param {function({progress:Number,rate:Number,loaded:Number,total:Number})} onProgress 
     * @returns {Promise}
     */
    addFile: (file, name, desc, isPrivate, isHidden, allowComments, allowReactions, onProgress) => api.postForm(`${config.apiroot}/videos`, {
        file: file,
        name: name ?? '',
        description: desc ?? '',
        isPrivate: isPrivate ?? false,
        isHidden: isHidden ?? false,
        allowComments: allowComments ?? false,
        allowReactions: allowReactions ?? false
    }, {
        onUploadProgress: onProgress,
        onDownloadProgress: onProgress
    }),

    /**
     * 
     * @param {String} url 
     * @param {String} maxDuration 
     * @param {String} name 
     * @param {String} desc 
     * @param {Boolean} isPrivate 
     * @param {Boolean} isHidden 
     * @param {Boolean} allowComments 
     * @param {Boolean} allowReactions 
     * @returns {Promise}
     */
    addUrl: (url, maxDuration, name, desc, isPrivate, isHidden, allowComments, allowReactions) => api.postForm(`${config.apiroot}/videos/url`, {
        url: url,
        maxDuration: maxDuration ?? '04:00:00.000',
        name: name ?? '',
        description: desc ?? '',
        isPrivate: isPrivate ?? false,
        isHidden: isHidden ?? false,
        allowComments: allowComments ?? false,
        allowReactions: allowReactions ?? false
    }),

    /**
     * 
     * @param {String} token 
     * @returns {Promise}
     */
    delete: (token) => api.delete(`${config.apiroot}/videos/${token}`),

    /**
     * 
     * @param {String} token 
     * @param {String} name 
     * @param {Number} userId 
     * @returns {Promise}
     */
    getAllProgresses: (name, userId, page, perPage, sortBy, sortDir) => api.get(`${config.apiroot}/videos/processing-all`, {
        params: {
            name: name ?? null,
            userId: userId ?? 0,
            page: page > 0 ? page : 1,
            perPage: perPage > 0 ? perPage : 25,
            sortBy: sortBy ?? null,
            sortDirection: sortDir ?? null
        }
    }),

    /**
     * 
     * @param {String} token 
     * @returns {Promise}
     */
    getProgress: (token) => api.get(`${config.apiroot}/videos/${token ?? ''}/processing`)
};
