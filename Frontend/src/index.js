import 'core-js';
import './utils/Extentions';
import './index.css';
import ReactDOM from 'react-dom/client';
import App from './App';
import * as Sentry from '@sentry/browser';
import config from './config.json';
import Common from './utils/Common';
import i18next from 'i18next';
import lang from './lang.json';
import { I18nextProvider } from 'react-i18next';

i18next.init({
    interpolation: {
        escapeValue: false,
    },
    lng: Common.getBrowserLocale(),
    fallbackLng: Common.getFallbackLocale(),
    resources: lang
});

Sentry.init({
    ...config.sentryConfig,
    environment: process.env.NODE_ENV
});

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
    <I18nextProvider i18n={i18next}>
        <App />
    </I18nextProvider>
);
