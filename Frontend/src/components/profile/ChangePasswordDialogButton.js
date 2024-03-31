import { Button, Dialog, DialogContent, DialogTitle, Typography } from '@mui/material';
import LockIcon from '@mui/icons-material/Lock';
import { useTranslation } from 'react-i18next';
import { useState } from 'react';
import PopupTransition from '../../ui/PopupTransition';

export default function ChangePasswordDialogButton() {
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
                <LockIcon />
                <Typography variant='button'>{t('user.profile.changepassword')}</Typography>
            </Button>
            <Dialog
                open={visible ?? false}
                TransitionComponent={PopupTransition}
                keepMounted
                onClose={onClose}
                fullWidth>
                <DialogTitle>{t('user.profile.changepassword.caption')}</DialogTitle>
                <DialogContent >

                </DialogContent>
            </Dialog>
        </div>
    )
}