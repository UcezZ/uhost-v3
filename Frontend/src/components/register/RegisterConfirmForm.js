import React, { useContext, useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Box from '@mui/material/Box';
import StateContext from '../../utils/StateContext';
import { CircularProgress, FormControl, Grid, InputLabel, MenuItem, Select, Typography } from '@mui/material';
import Common from '../../utils/Common';
import Validation from '../../utils/Validation';
import Styles from '../../ui/Styles';
import { useTranslation } from 'react-i18next';
import RegisterEndpoint from '../../api/RegisterEndpoint';

export default function RegisterConfirmForm({ next, prev }) {
    const { t } = useTranslation();
    const { setError } = useContext(StateContext);
    const [loading, setLoading] = useState(false);
    const [code, setCode] = useState('');

    async function onSubmit(event) {
        event?.preventDefault && event.preventDefault();

        setLoading(true);
        await RegisterEndpoint.confirm(code)
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

    function validateCode() {
        return code?.length > 5;
    }

    function isValid() {
        return validateCode();
    }

    return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
            }}
        >
            <Typography textAlign='justify' width='100%' >{t('register.checkcode')}</Typography>
            <Box
                component='form'
                noValidate
                onSubmit={onSubmit}
                sx={{ mt: 1, width: '100%' }}>
                <TextField
                    margin='normal'
                    fullWidth
                    required
                    label={t('register.code')}
                    error={!validateCode()}
                    disabled={loading}
                    value={code}
                    onChange={e => setCode(e.target.value)}
                    autoFocus
                    sx={{ textAlign: 'center' }}
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
                        {t('common.cancel')}
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