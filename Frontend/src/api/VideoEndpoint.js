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
    getByToken: (token) => api.get(`${config.apiroot}/videos/${token}`, {
        withCredentials: true,
        headers: {
            crossDomain: true
        }
    }),
    edit: (token, name, desc, isPrivate, isHidden, allowComments, allowReactions) => api.putForm(`${config.apiroot}/videos/${token}`, {
        name: name ?? '',
        description: desc ?? '',
        isPrivate: isPrivate ?? false,
        isHidden: isHidden ?? false,
        allowComments: allowComments ?? false,
        allowReactions: allowReactions ?? false
    }),
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
    delete: (token) => api.delete(`${config.apiroot}/videos/${token}`)
};
