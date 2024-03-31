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
                    setPassword('');
                    setPassword2('');
                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));
        setLoading(false);

        next && next(event);
    }

    function isValid() {
        return Validation.Video.name(name) && Validation.Video.desc(desc);
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
                    required
                    fullWidth
                    label={t('video.name')}
                    error={!Validation.Video.name(name)}
                    disabled={loading}
                    value={name}
                    onChange={e => setName(e.target.value)}
                    autoFocus
                />
                <TextField
                    margin='normal'
                    required
                    fullWidth
                    label={t('video.description')}
                    error={!Validation.Video.desc(desc)}
                    disabled={loading}
                    value={desc}
                    onChange={e => setDesc(e.target.value)}
                    minRows={3}
                    maxRows={10}
                    multiline
                />
                <FormControlLabel
                    control={<Checkbox color='primary' checked={isPrivate} onClick={e => {
                        setIsPrivate(!isPrivate);
                        if (!isPrivate) {
                            setIsHidden(true);
                        }
                    }} />}
                    label={t('video.isprivate')}
                    style={Styles.noSelectSx}
                />
                <FormControlLabel
                    control={<Checkbox color='primary' disabled={isPrivate} checked={isHidden} onClick={e => setIsHidden(!isHidden)} />}
                    label={t('video.ishidden')}
                    sx={Styles.noSelectSx}
                />
                <FormControlLabel
                    control={<Checkbox color='primary' checked={allowComments} onClick={e => setAllowComments(!allowComments)} />}
                    label={t('video.allowcomments')}
                    sx={Styles.noSelectSx}
                />
                <FormControlLabel
                    control={<Checkbox color='primary' checked={allowReactions} onClick={e => setAllowReactions(!allowReactions)} />}
                    label={t('video.allowreactions')}
                    sx={Styles.noSelectSx}
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