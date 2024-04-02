import { Button, Dialog, DialogContent, DialogTitle, Typography } from '@mui/material';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import { useTranslation } from 'react-i18next';
import { useState } from 'react';
import PopupTransition from '../../ui/PopupTransition';
import UploadAvatarForm from './UploadAvatarForm';

export default function UploadAvatarDialogButton({ shownUser, setShownUser }) {
    const { t } = useTranslation();
    const [visible, setVisible] = useState(false);

    function onClick() {
        setVisible(true);
    };
    function onClose() {
        setVisible(false);
    };

    return (
        <div style={{ flex: 1 }}>
            <Button
                size='large'
                variant='contained'
                onClick={onClick}
                fullWidth
                sx={{ gap: 1 }}>
                <CloudUploadIcon />
                <Typography variant='button'>{t('user.profile.avatar.upload')}</Typography>
            </Button>
            <Dialog
                open={visible ?? false}
                TransitionComponent={PopupTransition}
                keepMounted
                onClose={onClose}
                fullWidth>
                <DialogTitle>{t('user.profile.avatar.upload.caption')}</DialogTitle>
                <DialogContent >
                    <UploadAvatarForm visible={visible} next={onClose} shownUser={shownUser} setShownUser={setShownUser} />
                </DialogContent>
            </Dialog>
        </div>
    )
}