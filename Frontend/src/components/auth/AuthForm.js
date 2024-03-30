import React, { useContext, useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import FormControlLabel from '@mui/material/FormControlLabel';
import Checkbox from '@mui/material/Checkbox';
import Box from '@mui/material/Box';
import AuthEndpoint from '../../api/AuthEndpoint';
import StateContext from '../../utils/StateContext';
import { CircularProgress } from '@mui/material';
import Common from '../../utils/Common';
import Validation from '../../utils/Validation';
import Styles from '../../ui/Styles';

export default function AuthForm({ next, slim }) {
    const { setError, setUser } = useContext(StateContext);
    const [loading, setLoading] = useState(false);
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const [remember, setRemember] = useState(false);

    if (!next) {
        next = () => setLoading(false);
    }

    async function onSubmit(event) {
        event?.preventDefault && event.preventDefault();

        setLoading(true);

        await AuthEndpoint.login(login, password)
            .then(e => {
                if (e?.data?.success && e?.data?.result?.token) {
                    if (remember) {
                        localStorage.setItem('accessToken', e.data.result.token);
                    } else {
                        sessionStorage.setItem('accessToken', e.data.result.token);
                    }

                    next && next();
                    setUser(e?.data?.result?.user);
                }
                else {
                    showError(JSON.stringify(e.data));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));

        setLoading(false);
    }

    function isValid() {
        return Validation.Auth.login(login) && Validation.Auth.password(password);
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
                    error={!Validation.Auth.login(login)}
                    disabled={loading}
                    autoFocus
                    onChange={e => setLogin(e.target.value)}
                />
                <TextField
                    margin="normal"
                    required
                    fullWidth
                    name="password"
                    label="Пароль"
                    type="password"
                    id="password"
                    error={!Validation.Auth.password(password)}
                    disabled={loading}
                    autoComplete="current-password"
                    onChange={e => setPassword(e.target.value)}
                />
                <FormControlLabel
                    control={<Checkbox value="remember" color="primary" onChange={e => setRemember(e.target.value)} />}
                    label="Запомнить"
                    onClick={e => setRemember(e.target.value)}
                    sx={Styles.noSelectSx}
                />
                <Button
                    type="submit"
                    fullWidth
                    variant="contained"
                    disabled={loading || !isValid()}
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