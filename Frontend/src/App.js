import React, { useEffect, useState } from 'react';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import AuthPage from './components/pages/AuthPage';
import StateContext from './utils/StateContext';
import ErrorDialog from './components/ErrorDialog';
import Header from './components/header/Header';
import config from './config.json';
import { ThemeProvider } from '@mui/material/styles';
import AuthEndpoint from './api/AuthEndpoint';
import LoadingBox from './components/LoadingBox';
import Common from './utils/Common';
import { createTheme } from '@mui/material/styles';
import { green, lime, orange, red, yellow } from '@mui/material/colors';
import SearchPage from './components/pages/SearchPage';
import VideoPage from './components/pages/VideoPage';
import VideosPage from './components/pages/VideosPage';
import NotFoundPage from './components/pages/NotFoundPage';
import CssBaseline from '@mui/material/CssBaseline';
import MenuDrawer from './components/header/MenuDrawer';

export default function App() {
    const [error, setError] = useState(null);
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [isDrawerOpen, setIsDrawerOpen] = useState(false);

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

    const themeMode = Common.checkThemeName(user?.theme?.toLowerCase() ?? (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'));
    const themeIsDark = themeMode === 'dark';

    const theme = createTheme({
        palette: {
            primary: {
                main: themeIsDark
                    ? red[900]
                    : red[800]
            },
            secondary: {
                main: themeIsDark
                    ? orange[900]
                    : orange[800]
            },
            error: {
                main: themeIsDark
                    ? yellow[800]
                    : yellow[700]
            },
            success: {
                main: themeIsDark
                    ? green[800]
                    : green[700]
            },
            warning: {
                main: themeIsDark
                    ? lime['A700']
                    : lime['A400']
            },
            mode: themeMode
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
                <CssBaseline />
                <StateContext.Provider value={{
                    error, setError,
                    user, setUser,
                    isDrawerOpen, setIsDrawerOpen
                }}>
                    <BrowserRouter>
                        <MenuDrawer open={isDrawerOpen} />
                        <Header />
                        <Routes>
                            <Route path={`${config.webroot}/`} element={<SearchPage />} />
                            <Route path={`${config.webroot}/login`} element={<AuthPage />} />
                            <Route path={`${config.webroot}/video/:token`} element={<VideoPage />} />
                            <Route path={`${config.webroot}/videos`} element={<VideosPage />} />
                            <Route path='*' element={<NotFoundPage />} />
                        </Routes>
                    </BrowserRouter>
                    <ErrorDialog />
                </StateContext.Provider>
            </ThemeProvider>
        );
    }
}
