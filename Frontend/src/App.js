import React, { useEffect, useState } from "react";
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import AuthPage from "./components/pages/AuthPage";
import StateContext from './utils/StateContext';
import ErrorDialog from './components/ErrorDialog';
import Header from "./components/Header";
import config from './config.json';
import { ThemeProvider } from '@mui/material/styles';
import AuthEndpoint from "./api/AuthEndpoint";
import LoadingBox from "./components/LoadingBox";
import Common from "./utils/Common";
import { createTheme } from '@mui/material/styles';
import { orange, red } from '@mui/material/colors';
import SearchPage from "./components/pages/SearchPage";
import VideoPage from "./components/pages/VideoPage";
import NotFoundPage from "./components/pages/NotFoundPage";
import CssBaseline from '@mui/material/CssBaseline';

export default function App() {
    const [error, setError] = useState(null);
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    // загрузка пользователя
    useEffect(() => {
        if (loading && Common.isTokenPresent()) {
            AuthEndpoint.info()
                .then(e => {
                    if (e?.data?.success === true && e?.data?.result) {
                        setLoading(false);
                        setUser(e.data.result);
                    }
                })
                .catch(e => {
                    setLoading(false);
                    setUser();
                    Common.resetToken();
                });
        } else {
            setLoading(false);
        }
    }, [loading]);

    const theme = createTheme({
        palette: {
            primary: {
                main: red[800]
            },
            secondary: {
                main: orange[900]
            },
            mode: user?.theme?.toLowerCase() ?? (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light')
        },
    });

    if (loading) {
        return (
            <ThemeProvider theme={theme} >
                <LoadingBox />
            </ThemeProvider>
        );
    }
    else {
        return (
            <ThemeProvider theme={theme}>
                <StateContext.Provider value={{
                    error, setError,
                    user, setUser
                }}>
                    <BrowserRouter>
                        <CssBaseline />
                        <Header />
                        <Routes>
                            <Route path={`${config.webroot}/`} element={<SearchPage />} />
                            <Route path={`${config.webroot}/login`} element={<AuthPage />} />
                            <Route path={`${config.webroot}/video/:token`} element={<VideoPage />} />
                            <Route path='*' element={<NotFoundPage />} />
                        </Routes>
                    </BrowserRouter>
                    <ErrorDialog />
                </StateContext.Provider>
            </ThemeProvider>
        );
    }
}
