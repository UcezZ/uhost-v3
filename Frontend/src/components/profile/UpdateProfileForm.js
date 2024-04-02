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
import UserEndpoint from '../../api/UserEndpoint';

export default function UpdateProfileForm({ shownUser, next }) {
    const { t } = useTranslation();
    const { setError, setUser } = useContext(StateContext);
    const [loading, setLoading] = useState(false);
    const [name, setName] = useState(shownUser?.name ?? '');
    const [desc, setDesc] = useState(shownUser?.description ?? '');
    const [theme, setTheme] = useState(Common.checkThemeName(shownUser?.theme));
    const [locale, setLocale] = useState(Common.filterLocale(shownUser?.locale));

    async function onSubmit(event) {
        event?.preventDefault && event.preventDefault();

        setLoading(true);
        await UserEndpoint.updateSelf(name, desc, theme.toPascalCase(), locale.toPascalCase())
            .then(e => {
                if (e?.data?.success && e?.data?.result) {
                    setUser && setUser(e.data.result);
                    e.data.result?.name && setName(e.data.result.name);
                    e.data.result?.description && setDesc(e.data.result.description);
                    e.data.result?.theme && setTheme(Common.checkThemeName(e.data.result.theme));
                    e.data.result?.locale && setLocale(Common.filterLocale(e.data.result.locale));

                    next && next();
                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));
        setLoading(false);
    }

    function isValid() {
        return Validation.User.name(name) && Validation.User.desc(desc);
    }

    return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
            }}
        >
            <Box component='form' noValidate onSubmit={onSubmit} sx={{ mt: 1 }}>
                <TextField
                    margin='normal'
                    fullWidth
                    label={t('user.name')}
                    error={!Validation.User.name(name)}
                    disabled={loading}
                    value={name}
                    onChange={e => setName(e.target.value)}
                    autoFocus
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
                        onClick={next}
                    >
                        {t('common.cancel')}
                    </Button>
                    <Button
                        type='submit'
                        fullWidth
                        variant='contained'
                        disabled={loading || !isValid()}
                        sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                    >
                        {loading ? <CircularProgress size={20} /> : t('common.apply')}
                    </Button>
                </Box>
            </Box>
        </Box>
    );
}