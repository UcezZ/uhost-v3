import api from './api';
import config from '../config.json';

export default {
    /**
     * 
     * @param {String} email 
     * @param {String} login 
     * @param {String} theme 
     * @param {String} locale 
     * @param {String} name 
     * @param {String} description 
     * @param {String} password 
     * @param {String} passwordConfirm 
     * @returns 
     */
    register: (email, login, theme, locale, name, description, password, passwordConfirm) => api.postForm(`${config.apiroot}/register`, {
        email,
        login,
        theme,
        locale,
        name,
        description,
        password,
        passwordConfirm
    }),

    /**
     * 
     * @param {String} code 
     * @returns 
     */
    confirm: (code) => api.post(`${config.apiroot}/register/${code}`)
};
