import ReactDOM from 'react-dom/client';
import App from './App';
import 'core-js';
import './index.css';
import ApplyExtentions from './utils/Extentions';

const root = ReactDOM.createRoot(document.getElementById('root'));
ApplyExtentions();
root.render(<App />);
