import React, { useContext, useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Box from '@mui/material/Box';
import StateContext from '../../utils/StateContext';
import { CircularProgress, FormControl, Grid, InputLabel, MenuItem, Select } from '@mui/material';
import Common from '../../utils/Common';
import Validation from '../../utils/Validation';
import Styles from '../../ui/Styles';
import { useTranslation } from 'react-i18next';
import RegisterEndpoint from '../../api/RegisterEndpoint';

export default function RegisterForm({ next, prev }) {
    const { t } = useTranslation();
    const { setError } = useContext(StateContext);
    const [loading, setLoading] = useState(false);
    const [name, setName] = useState('');
    const [desc, setDesc] = useState('');
    const [email, setEmail] = useState('');
    const [login, setLogin] = useState('');
    const [theme, setTheme] = useState(Common.getBrowserTheme());
    const [locale, setLocale] = useState(Common.getBrowserLocale());
    const [password, setPassword] = useState('');
    const [password2, setPassword2] = useState('');

    async function onSubmit(event) {
        event?.preventDefault && event.preventDefault();

        setLoading(true);
        await RegisterEndpoint.register(email, login, theme, locale, name, desc, password, password2)
            .then(e => {
                if (e?.data?.success && e?.data?.result) {
                    next && next();
                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));
        setLoading(false);
    }

    function onBack() {
        prev && prev();
    }

    function validatePassword2() {
        return Validation.User.password(password2) && password === password2;
    }

    function isValid() {
        return Validation.User.login(login) &&
            Validation.User.name(name) &&
            Validation.User.email(email) &&
            Validation.User.desc(desc) &&
            Validation.User.password(password) &&
            validatePassword2();
    }

    return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
            }}
        >
            <Box
                component='form'
                noValidate
                onSubmit={onSubmit}
                sx={{ mt: 1, width: '100%' }}>
                <TextField
                    margin='normal'
                    fullWidth
                    required
                    label={t('user.login')}
                    error={!Validation.User.login(login)}
                    disabled={loading}
                    value={login}
                    onChange={e => setLogin(e.target.value)}
                    autoFocus
                />
                <TextField
                    margin='normal'
                    fullWidth
                    required
                    label={t('user.email')}
                    error={!Validation.User.email(email)}
                    disabled={loading}
                    value={email}
                    onChange={e => setEmail(e.target.value)}
                />
                <TextField
                    margin='normal'
                    fullWidth
                    label={t('user.name')}
                    error={!Validation.User.name(name)}
                    disabled={loading}
                    value={name}
                    onChange={e => setName(e.target.value)}
                />
                <TextField
                    margin='normal'
                    fullWidth
                    label={t('user.description')}
                    error={!Validation.User.desc(desc)}
                    disabled={loading}
                    value={desc}
                    onChange={e => setDesc(e.target.value)}
                    minRows={3}
                    maxRows={10}
                    multiline
                />
                <Grid container columnSpacing={2} mt={1} >
                    <Grid item xs={6}>
                        <FormControl sx={{ width: '100%' }} >
                            <InputLabel htmlFor='theme'>{t('user.theme')}</InputLabel>
                            <Select
                                id='theme'
                                label={t('user.theme')}
                                value={theme}
                                onChange={e => setTheme(e.target.value)}
                            >
                                {Common.getThemes().map((e, i) => <MenuItem value={e} key={i}>{t(`user.theme.${e}`)}</MenuItem>)}
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid item xs={6}>
                        <FormControl sx={{ width: '100%' }} >
                            <InputLabel htmlFor='locale'>{t('user.locale')}</InputLabel>
                            <Select
                                id='locale'
                                label={t('user.locale')}
                                value={locale}
                                onChange={e => setLocale(e.target.value)}
                            >
                                {Common.getLocales().map((e, i) => <MenuItem value={e} key={i}>{t(`user.locale.${e}`)}</MenuItem>)}
                            </Select>
                        </FormControl>
                    </Grid>
                </Grid>
                <TextField
                    margin='normal'
                    type='password'
                    required
                    fullWidth
                    label={t('user.password')}
                    error={!Validation.User.password(password)}
                    disabled={loading}
                    value={password}
                    onChange={e => setPassword(e.target.value)}
                />
                <TextField
                    margin='normal'
                    type='password'
                    required
                    fullWidth
                    label={t('user.password2')}
                    error={!validatePassword2()}
                    disabled={loading}
                    value={password2}
                    onChange={e => setPassword2(e.target.value)}
                />
                <Box sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    alignItems: 'center',
                    gap: 2,
                    ...Styles.noSelectSx
                }}>
                    <Button
                        fullWidth
                        variant='outlined'
                        disabled={loading}
                        sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                        onClick={onBack}
                    >
                        {t('common.back')}
                    </Button>
                    <Button
                        type='submit'
                        fullWidth
                        variant='contained'
                        disabled={loading || !isValid()}
                        sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                    >
                        {loading ? <CircularProgress size={20} /> : t('common.next')}
                    </Button>
                </Box>
            </Box>
        </Box>
    );
}