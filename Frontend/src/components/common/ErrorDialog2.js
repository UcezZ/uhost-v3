import { useState } from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import PopupTransition from '../../ui/PopupTransition';
import { useTranslation } from 'react-i18next';

export default function ErrorDialog2({ message, onClose }) {
    const { t } = useTranslation();
    const [visible, setVisible] = useState(true);

    const handleClose = () => {
        setVisible(false);
        onClose && onClose();
    };

    return (
        <Dialog
            open={visible}
            TransitionComponent={PopupTransition}
            keepMounted
            onClose={handleClose}
            sx={{ zIndex: 1333 }}
        >
            <DialogTitle>{t('common.warn')}</DialogTitle>
            <DialogContent color='red'>
                <DialogContentText>{message}</DialogContentText>
            </DialogContent>
            <DialogActions>
                <Button onClick={handleClose}>{t('common.close')}</Button>
            </DialogActions>
        </Dialog>
    );
}