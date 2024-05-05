import api from './api';
import config from '../config.json';

export default {

    /**
     * 
     * @param {Number} page 
     * @param {Number} perPage 
     * @param {String} sortBy
     * @param {String} sortDir
     * @returns {Promise}
     */
    getAll: (page, perPage, sortBy, sortDir) => api.get(`${config.apiroot}/users`, {
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
    getById: (id) => api.get(`${config.apiroot}/users/${id}`),

    /**
     * 
     * @param {String} login 
     * @returns {Promise}
     */
    getByLogin: (login) => api.get(`${config.apiroot}/users/by-login/${login}`),

    /**
     * 
     * @param {String} name 
     * @param {String} desc 
     * @param {String} theme 
     * @param {String} locale 
     * @returns {Promise}
     */
    updateSelf: (name, desc, theme, locale) => api.putForm(`${config.apiroot}/users`, {
        name: name ?? '',
        desc: desc ?? '',
        theme: theme ?? '',
        locale: locale ?? ''
    }),

    /**
     * 
     * @param {Number} id
     * @param {String} login 
     * @param {String} email
     * @param {String} name 
     * @param {String} desc 
     * @param {String} theme 
     * @param {String} locale 
     * @returns {Promise}
     */
    update: (id, login, email, roles, name, desc, theme, locale) => api.put(
        `${config.apiroot}/users/${id}`,
        JSON.stringify({
            login,
            email,
            roleIds: roles,
            name: name ?? '',
            desc: desc ?? '',
            theme: theme ?? '',
            locale: locale ?? ''
        }),
        {
            headers: {
                'Content-Type': 'application/json'
            }
        }),

    /**
     * 
     * @param {String} password 
     * @param {String} passwordConfirm 
     * @returns 
     */
    changePasswordSelf: (password, passwordConfirm) => api.putForm(`${config.apiroot}/users/password`, {
        password: password ?? '',
        passwordConfirm: passwordConfirm ?? passwordConfirm
    }),

    /**
     * 
     * @param {Number} id
     * @param {String} password 
     * @param {String} passwordConfirm 
     * @returns 
     */
    changePasswordSelf: (id, password, passwordConfirm) => api.putForm(`${config.apiroot}/users/password/${id}`, {
        password: password ?? '',
        passwordConfirm: passwordConfirm ?? passwordConfirm
    }),

    /**
     * 
     * @param {any} file 
     * @returns {Promise}
     */
    uploadAvatarSelf: (file) => api.postForm(`${config.apiroot}/users/avatar`, {
        file
    }),

    /**
     * 
     * @returns {Promise}
     */
    deleteAvatarSelf: () => api.delete(`${config.apiroot}/users/avatar`)
}