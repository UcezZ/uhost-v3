import 'core-js';
import './utils/Extentions';
import './index.css';
import ReactDOM from 'react-dom/client';
import App from './App';
import { init } from '@sentry/browser';
import config from './config.json';

init(config.sentryConfig);
const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(<App />);
