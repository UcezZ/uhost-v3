import React, { useEffect, useState } from 'react';
import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import AuthPage from './components/pages/AuthPage';
import StateContext from './utils/StateContext';
import ErrorDialog from './components/common/ErrorDialog';
import Header from './components/header/Header';
import config from './config.json';
import { ThemeProvider } from '@mui/material/styles';
import AuthEndpoint from './api/AuthEndpoint';
import LoadingBox from './components/common/LoadingBox';
import Common from './utils/Common';
import { createTheme } from '@mui/material/styles';
import { green, lime, orange, red, yellow } from '@mui/material/colors';
import SearchPage from './components/pages/HomePage';
import VideoPage from './components/pages/VideoPage';
import VideosPage from './components/pages/VideosPage';
import NotFoundPage from './components/pages/NotFoundPage';
import CssBaseline from '@mui/material/CssBaseline';
import MenuDrawer from './components/header/MenuDrawer';
import VideoProcessingPage from './components/pages/VideoProcessingPage';
import { useTranslation } from 'react-i18next';
import ProfilePage from './components/pages/ProfilePage';
import Rights from './utils/Rights';
import LogsPage from './components/pages/LogsPage';
import SessionPage from './components/pages/SessionsPage';
import RegisterPage from './components/pages/RegisterPage';
import RolePage from './components/pages/RolePage';
import UserPage from './components/pages/UserPage';
import ErrorBoundary from './components/common/ErrorBoundary';
import ErrorDialog2 from './components/common/ErrorDialog2';

function ThworsError() {
    throw new Error('throw-error route');
}

export default function App() {
    const { i18n, t } = useTranslation();
    const [error, setError] = useState(null);
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [isDrawerOpen, setIsDrawerOpen] = useState(false);

    async function onLoad() {
        if (loading && Common.isTokenPresent()) {
            await AuthEndpoint.info()
                .then(e => {
                    if (e?.data?.success === true && e?.data?.result) {
                        setUser(e.data.result);
                    }
                })
                .catch(e => {
                    setUser();
                    Common.resetToken();
                });
        }

        setLoading(false);
    }

    // загрузка пользователя
    useEffect(() => {
        onLoad();
    }, [loading]);

    // меняем локаль
    useEffect(() => {
        i18n.changeLanguage(Common.getLocale(user));
    }, [user]);

    const themeMode = Common.checkThemeName(user?.theme?.toLowerCase() ?? Common.getBrowserTheme());
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

    return (
        <ThemeProvider theme={theme}>
            <CssBaseline />
            <ErrorBoundary>
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
                            <Route path={`${config.webroot}/profile/:login`} element={<ProfilePage />} />
                            <Route path={`${config.webroot}/videos/:login`} element={<VideosPage />} />

                            {!user?.id && <Route path={`${config.webroot}/register`} element={<RegisterPage />} />}

                            {user?.id > 0 && <Route path={`${config.webroot}/videos`} element={<VideosPage />} />}
                            {user?.id > 0 && <Route path={`${config.webroot}/profile`} element={<ProfilePage />} />}
                            {user?.id > 0 && <Route path={`${config.webroot}/video-processing`} element={<VideoProcessingPage />} />}
                            {user?.id > 0 && <Route path={`${config.webroot}/video-processing/:token`} element={<VideoProcessingPage />} />}

                            {Rights.checkAnyRight(user, Rights.AdminLogAccess) && <Route path={`${config.webroot}/admin/logs`} element={<LogsPage />} />}
                            {Rights.checkAnyRight(user, Rights.AdminSessionAccess, Rights.AdminSessionTerminate) && <Route path={`${config.webroot}/admin/sessions`} element={<SessionPage />} />}
                            {Rights.checkAnyRight(user, Rights.RoleCreateUpdate, Rights.RoleDelete) && <Route path={`${config.webroot}/admin/roles`} element={<RolePage />} />}
                            {Rights.checkAnyRight(user, Rights.UserCreate, Rights.UserDelete, Rights.UserInteractAll) && <Route path={`${config.webroot}/admin/users`} element={<UserPage />} />}

                            <Route path={`${config.webroot}/throw-error`} element={<ThworsError />} />

                            <Route path='*' element={<NotFoundPage />} />
                        </Routes>
                    </BrowserRouter>
                    <ErrorDialog />
                    {Common.checkSetSafariExplorerAlert() && <ErrorDialog2 message={t('common.safariexploreralert')} onClose={Common.setSafariExplorerAlert} />}
                </StateContext.Provider>
            </ErrorBoundary>
        </ThemeProvider>
    );
}
