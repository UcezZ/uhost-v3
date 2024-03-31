import api from './api';
import config from '../config.json';

export default {

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
     * @param {String} password 
     * @param {String} passwordConfirm 
     * @returns 
     */
    changePasswordSelf: (password, passwordConfirm) => api.putForm(`${config.apiroot}/users`, {
        password: password ?? '',
        passwordConfirm: passwordConfirm ?? passwordConfirm
    })
}