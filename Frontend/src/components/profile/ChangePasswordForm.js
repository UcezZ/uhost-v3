import React, { useContext, useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import FormControlLabel from '@mui/material/FormControlLabel';
import Checkbox from '@mui/material/Checkbox';
import Box from '@mui/material/Box';
import StateContext from '../../utils/StateContext';
import { CircularProgress } from '@mui/material';
import Common from '../../utils/Common';
import VideoEndpoint from '../../api/VideoEndpoint';
import Validation from '../../utils/Validation';
import Styles from '../../ui/Styles';
import { useTranslation } from 'react-i18next';
import UserEndpoint from '../../api/UserEndpoint';

export default function ChangePasswordForm({ next }) {
    const { t } = useTranslation();
    const { setError } = useContext(StateContext);
    const [loading, setLoading] = useState(false);
    const [password, setPassword] = useState('');
    const [password2, setPassword2] = useState('');

    async function onSubmit(event) {
        event?.preventDefault && event.preventDefault();

        setLoading(true);
        await UserEndpoint.changePasswordSelf(password, password2)
            .then(e => {
                if (e?.data?.success && e?.data?.result) {
                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));
        setLoading(false);

        setPassword('');
        setPassword2('');

        next && next(event);
    }

    function isValid() {
        return Validation.User.password(password) && Validation.User.password(password2) && password === password2;
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
                width='100%'
                component='form'
                noValidate
                onSubmit={onSubmit}
                sx={{ mt: 1 }}>
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
                    autoFocus
                />
                <TextField
                    margin='normal'
                    type='password'
                    required
                    fullWidth
                    label={t('user.password2')}
                    error={!Validation.User.password(password2)}
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