import React, { useContext, useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import FormControlLabel from '@mui/material/FormControlLabel';
import Checkbox from '@mui/material/Checkbox';
import Box from '@mui/material/Box';
import AuthEndpoint from '../../api/AuthEndpoint';
import StateContext from '../../utils/StateContext';
import { CircularProgress, Grid, Typography } from '@mui/material';
import Common from '../../utils/Common';
import Validation from '../../utils/Validation';
import Styles from '../../ui/Styles';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import config from '../../config.json';

export default function AuthForm({ next, slim }) {
    const { t } = useTranslation();
    const { setError, setUser } = useContext(StateContext);
    const [loading, setLoading] = useState(false);
    const [login, setLogin] = useState('');
    const [password, setPassword] = useState('');
    const [remember, setRemember] = useState(true);

    async function onSubmit(event) {
        event?.preventDefault && event.preventDefault();

        setLoading(true);

        await AuthEndpoint.login(login, password)
            .then(e => {
                if (e?.data?.success && e?.data?.result?.token) {
                    try {
                        if (remember) {
                            localStorage.setItem(Common.getTokenKey(), e.data.result.token);
                            localStorage.getItem(Common.getTokenKey());
                        } else {
                            sessionStorage.setItem(Common.getTokenKey(), e.data.result.token);
                            sessionStorage.getItem(Common.getTokenKey());
                        }
                    } catch {
                        setError(t('common.storageerror'));
                        return;
                    }

                    next && next();

                    setLogin('');
                    setPassword('');
                    setUser(e?.data?.result?.user);
                }
                else {
                    setError(Common.transformErrorData(e));
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
            <Box component='form' noValidate onSubmit={onSubmit} sx={{ mt: 1 }}>
                <TextField
                    margin='normal'
                    required
                    fullWidth
                    id='login'
                    label={t('auth.username')}
                    name='login'
                    autoComplete='login'
                    error={!Validation.Auth.login(login)}
                    disabled={loading}
                    value={login}
                    autoFocus
                    onChange={e => setLogin(e.target.value)}
                />
                <TextField
                    margin='normal'
                    required
                    fullWidth
                    name='password'
                    label={t('auth.password')}
                    type='password'
                    id='password'
                    error={!Validation.Auth.password(password)}
                    value={password}
                    disabled={loading}
                    autoComplete='current-password'
                    onChange={e => setPassword(e.target.value)}
                />
                <FormControlLabel
                    control={<Checkbox checked={remember} color='primary' onChange={e => setRemember(!remember)} />}
                    label={t('auth.remember')}
                    sx={Styles.noSelectSx}
                />
                <Button
                    type='submit'
                    fullWidth
                    variant='contained'
                    disabled={loading || !isValid()}
                    sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                >
                    {loading ? <CircularProgress size={20} /> : t('auth.login')}
                </Button>
                <Grid container>
                    {/* <Grid item xs>
                        <Link href='#' variant='body2'>
                            Forgot password?
                        </Link>
                    </Grid> */}
                    <Grid item xs textAlign='center' >
                        <Typography variant='body2' component='span'>{t('register.suggest')}</Typography>
                        <span> </span>
                        <Link to={`${config.webroot}/register`} onClick={next}>
                            <Typography variant='body2' component='span'>{t('register.go')}</Typography>
                        </Link>
                    </Grid>
                </Grid>
            </Box>
        </Box>
    );
}