import React, { useContext, useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import FormControlLabel from '@mui/material/FormControlLabel';
import Checkbox from '@mui/material/Checkbox';
import Link from '@mui/material/Link';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import AuthEndpoint from '../../api/AuthEndpoint';
import StateContext from '../../utils/StateContext';
import { CircularProgress } from '@mui/material';
import Common from '../../utils/Common';

export default function AuthForm({ next, slim }) {
    const { setError, setUser } = useContext(StateContext);
    const [loading, setLoading] = useState(false);
    const [loginError, setLoginError] = useState(false);
    const [passwordError, setPasswordError] = useState(false);

    var remember = false;

    if (!next) {
        next = () => setLoading(false);
    }

    /**
     * 
     * @param {FormData} data 
     */
    function validate(data) {
        var login = data.get('login').length < 3;
        if (login) {
            setLoginError(true);
        }
        var password = data.get('password').length < 3;
        if (password) {
            setPasswordError(true);
        }

        return !login && !password;
    }

    async function onSubmit(event) {
        event?.preventDefault && event.preventDefault();
        var data = new FormData(event.target);

        if (!validate(data)) {
            return;
        }

        setLoading(true);

        await AuthEndpoint.login(data)
            .then(e => {
                if (e?.data?.success && e?.data?.result?.token) {
                    if (remember) {
                        localStorage.setItem('accessToken', e.data.result.token);
                    } else {
                        sessionStorage.setItem('accessToken', e.data.result.token);
                    }

                    next && next();
                    setLoading(false);
                    setUser(e?.data?.result?.user);
                }
                else {
                    showError(JSON.stringify(e.data));
                }
            })
            .catch(e => { showError(Common.transformErrorData(e)) });
    }

    function showError(e) {
        setLoading(false);
        setError(e);
    }

    async function onRememberClick(event) {
        remember = event.target.value;
    }

    function onLoginChanged(e) {
        if (loginError) {
            setLoginError();
        }
    }

    function onPasswordChanged(e) {
        if (passwordError) {
            setPasswordError();
        }
    }

    return (
        <Box
            sx={{
                marginTop: slim ? 0 : 8,
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
            }}
        >
            <Box component="form" noValidate onSubmit={onSubmit} sx={{ mt: 1 }}>
                <TextField
                    margin="normal"
                    required
                    fullWidth
                    id="login"
                    label="Логин"
                    name="login"
                    autoComplete="login"
                    error={loginError}
                    disabled={loading}
                    autoFocus
                    onChange={onLoginChanged}
                />
                <TextField
                    margin="normal"
                    required
                    fullWidth
                    name="password"
                    label="Пароль"
                    type="password"
                    id="password"
                    error={passwordError}
                    disabled={loading}
                    autoComplete="current-password"
                    onChange={onPasswordChanged}
                />
                <FormControlLabel
                    control={<Checkbox value="remember" color="primary" />}
                    label="Запомнить"
                    onClick={onRememberClick}
                />
                <Button
                    type="submit"
                    fullWidth
                    variant="contained"
                    disabled={loading}
                    sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                >
                    {loading ? <CircularProgress size={20} /> : 'Войти'}
                </Button>
                {/* <Grid container>
                    <Grid item xs>
                        <Link href="#" variant="body2">
                            Forgot password?
                        </Link>
                    </Grid>
                    <Grid item>
                        <Link href="#" variant="body2">
                            {"Don't have an account? Sign Up"}
                        </Link>
                    </Grid>
                </Grid> */}
            </Box>
        </Box>
    );
}