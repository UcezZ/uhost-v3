import axios from 'axios';
import Common from '../utils/Common';
import * as Sentry from '@sentry/browser';
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
        try {
            var token = sessionStorage.getItem(Common.getTokenKey());
            if (token) {
                cfg.headers.Authorization = `Bearer ${token}`;
            } else {
                token = localStorage.getItem(Common.getTokenKey());

                if (token) {
                    cfg.headers.Authorization = `Bearer ${token}`;
                }
            }
        } catch (err) {
            Sentry.withScope(scope => {
                scope.setExtra('msg', { message: 'failed to get token from storage in axios' });
                Sentry.captureException(err);
            });
        }

        return cfg;
    },
    (error) => Promise.reject(error)
);

export default api;
