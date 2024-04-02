import { useContext, useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Box from '@mui/material/Box';
import StateContext from '../../utils/StateContext';
import { CircularProgress, Divider, IconButton } from '@mui/material';
import Common from '../../utils/Common';
import { useTranslation } from 'react-i18next';
import UserEndpoint from '../../api/UserEndpoint';
import CloseIcon from '@mui/icons-material/Close';
import AttachFileIcon from '@mui/icons-material/AttachFile';
import { setUser } from '@sentry/browser';

const MAX_FILE_SIZE = 5242880;

export default function UploadAvatarForm({ next, shownUser, setShownUser }) {
    const { t } = useTranslation();
    const { setError, user, setuser } = useContext(StateContext);
    const [loading, setLoading] = useState(false);
    const [imageFile, setImageFile] = useState(null);

    async function onSubmit(event) {
        event?.preventDefault && event.preventDefault();

        setLoading(true);
        await UserEndpoint.uploadAvatarSelf(imageFile)
            .then(e => {
                if (e?.data?.success && e?.data?.result?.length) {
                    setImageFile();
                    user && setUser && setUser({ ...user, avatarUrl: e.data.result });
                    setShownUser && shownUser && setShownUser({ ...shownUser, avatarUrl: e.data.result });
                    next && next(event);
                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));
        setLoading(false);
    }

    function onFileSelected(e) {
        var selectedFile = e?.target?.files?.item && e.target.files.item(0);

        if (selectedFile !== imageFile) {
            setImageFile(selectedFile);
        }
    }

    function isValid() {
        return Boolean(imageFile);
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
                {
                    imageFile && <div style={{ display: 'flex', gap: '0.5em', marginBottom: '0.5em' }}>
                        <TextField
                            fullWidth
                            label={t('common.selectedfile')}
                            defaultValue={imageFile?.name}
                            disabled
                            variant='outlined'
                        />
                        <TextField
                            label={t('common.filesize')}
                            defaultValue={Common.sizeToHuman(imageFile?.size)}
                            disabled
                            variant='outlined'
                        />
                    </div>
                }
                <div style={{ display: 'flex', gap: '0.5em' }}>
                    <Button
                        fullWidth
                        disabled={loading}
                        component='label'
                        variant='contained'
                        startIcon={<AttachFileIcon sx={{ height: 40, width: 40 }} />}
                        sx={{ minHeight: 40 }}>
                        {t('common.fileupload')}
                        <input
                            maxLength={MAX_FILE_SIZE}
                            type='file'
                            hidden={true}
                            onChange={onFileSelected}
                            accept='image/bmp, image/gif, image/jpeg, image/pjpeg, image/png'
                            style={{
                                clip: 'rect(0 0 0 0)',
                                clipPath: 'inset(50%)',
                                height: 1,
                                overflow: 'hidden',
                                position: 'absolute',
                                bottom: 0,
                                left: 0,
                                whiteSpace: 'nowrap',
                                width: 1,
                            }} />
                    </Button>
                    {
                        imageFile && !loading && <IconButton
                            sx={{ height: 48, width: 48 }}
                            onClick={e => onFileSelected()} >
                            <CloseIcon />
                        </IconButton>
                    }
                </div>
                <div style={{ display: 'flex', gap: '0.5em' }}>
                    {
                        !loading && <Button
                            fullWidth
                            variant='outlined'
                            disabled={loading}
                            sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                            onClick={next}
                        >
                            {t('common.cancel')}
                        </Button>
                    }
                    <Button
                        type='submit'
                        fullWidth
                        variant='contained'
                        disabled={loading || !isValid()}
                        sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                    >
                        {loading ? <CircularProgress size={20} /> : t('user.profile.avatar.upload.submit')}
                    </Button>
                </div>
            </Box>
        </Box>
    );
}