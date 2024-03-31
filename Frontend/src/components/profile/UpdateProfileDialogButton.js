import { Button, Dialog, DialogContent, DialogTitle, Typography } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import { useTranslation } from 'react-i18next';
import { useState } from 'react';
import PopupTransition from '../../ui/PopupTransition';

export default function UpdateProfileDialogButton({ id }) {
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
                <EditIcon />
                <Typography variant='button'>{t('user.profile.edit')}</Typography>
            </Button>
            <Dialog
                open={visible ?? false}
                TransitionComponent={PopupTransition}
                keepMounted
                onClose={onClose}
                fullWidth>
                <DialogTitle>{t('user.profile.edit.caption')}</DialogTitle>
                <DialogContent >

                </DialogContent>
            </Dialog>
        </div>
    )
}