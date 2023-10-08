import React, { useState } from "react";
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import AuthPage from "./components/pages/AuthPage";
import StateContext from './context/StateContext';
import ErrorDialog from './components/ErrorDialog';
import Header from "./components/Header";
import config from './config.json';

export default function App() {
  const [theme, setTheme] = useState('dark');
  const [locale, setLocale] = useState('');
  const [error, setError] = useState();

  return (
    <StateContext.Provider value={{
      theme: theme, setTheme: setTheme,
      error: error, setError: setError
    }}>
      <Header />
      <BrowserRouter>
        <Routes>
          <Route path={`${config.webroot}/`} element={<Link to={`${config.webroot}/login`}>login</Link>} />
          <Route path={`${config.webroot}/login`} element={<AuthPage />} />
        </Routes>
      </BrowserRouter>
      <ErrorDialog />
    </StateContext.Provider>
  );
}
