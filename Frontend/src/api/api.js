import axios from 'axios';
//import { makeUrl, makeReqConfig } from './api-config.js';

//export const API_URL = 'http://localhost:5101';
//export const API_URL = process.env.VITE_HTTP_BACKEND_HOST;

// настройки axios
const api = axios.create({
    //baseURL: API_URL,
    //makeUrl,
    //headers: { 'Content-Type': 'application/json', Accept: 'application/json' },
    headers: { 'Content-Type': 'multipart/form-data' },
});

api.interceptors.request.use(
    (cfg) => {
        const local = localStorage.getItem('accessToken');
        const session = sessionStorage.getItem('accessToken');
        if (local) {
            cfg.headers.Authorization = `Bearer ${local}`;
        } else if (session) {
            cfg.headers.Authorization = `Bearer ${session}`;
        }

        return cfg;
    },
    (error) => Promise.reject(error)
);

export default api;
