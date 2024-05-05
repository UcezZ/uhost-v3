import React, { useContext, useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Box from '@mui/material/Box';
import StateContext from '../../utils/StateContext';
import { CircularProgress, Checkbox, ListItemText, FormControl, Grid, InputLabel, MenuItem, Select } from '@mui/material';
import Common from '../../utils/Common';
import Validation from '../../utils/Validation';
import Styles from '../../ui/Styles';
import { useTranslation } from 'react-i18next';
import UserEndpoint from '../../api/UserEndpoint';

export default function UserUpdateForm({ user, allRoles, next, onClose }) {
    const { t } = useTranslation();
    const { setError } = useContext(StateContext);
    const [loading, setLoading] = useState(false);
    const [name, setName] = useState(user?.name ?? '');
    const [desc, setDesc] = useState(user?.description ?? '');
    const [login, setLogin] = useState(user?.login ?? '');
    const [email, setEmail] = useState(user?.email ?? '');
    const [theme, setTheme] = useState(Common.checkThemeName(user?.theme));
    const [locale, setLocale] = useState(Common.filterLocale(user?.locale));
    const [roles, setRoles] = useState(user?.roleIds ?? []);

    async function onSubmit(event) {
        event?.preventDefault && event.preventDefault();

        setLoading(true);
        await UserEndpoint.update(user.id, login, email, roles, name, desc, theme, locale)
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

    function isValid() {
        return Validation.User.name(name) &&
            Validation.User.login(login) &&
            Validation.User.email(email) &&
            Validation.User.desc(desc);
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
                noValidate
                onSubmit={onSubmit}
                sx={{ mt: 1, width: '100%' }}>
                <TextField
                    margin='normal'
                    fullWidth
                    label={t('user.login')}
                    error={!Validation.User.login(login)}
                    disabled={loading}
                    value={login}
                    onChange={e => setLogin(e.target.value)}
                />
                <TextField
                    margin='normal'
                    fullWidth
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
                <FormControl sx={{ width: '100%', mt: 2 }}>
                    <InputLabel id='select-roles'>{t('user.roles')}</InputLabel>
                    <Select
                        labelId='select-roles'
                        multiple
                        value={roles}
                        onChange={e => setRoles(e.target.value)}
                        label={t('user.roles')}
                        renderValue={e => e.map(k => allRoles.firstOrDefault(r => r?.id === k)?.name).join('; ')}

                    >
                        {allRoles.map((e, i) => (
                            <MenuItem key={i} value={e?.id}>
                                <Checkbox checked={roles.includes(e?.id)} />
                                <ListItemText primary={e?.name} />
                            </MenuItem>
                        ))}
                    </Select>
                </FormControl>
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
                        onClick={onClose}
                    >
                        {t('common.cancel')}
                    </Button>
                    <Button
                        fullWidth
                        variant='contained'
                        disabled={loading || !isValid()}
                        sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                        onClick={onSubmit}
                    >
                        {loading ? <CircularProgress size={20} /> : t('common.apply')}
                    </Button>
                </Box>
            </Box>
        </Box>
    );
}