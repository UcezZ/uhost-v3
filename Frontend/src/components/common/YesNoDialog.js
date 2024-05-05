import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import { useTranslation } from 'react-i18next';
import PopupTransition from '../../ui/PopupTransition';

export default function YesNoDialog({ visible, setVisible, onYes, onNo, message }) {
    const { t } = useTranslation();

    function onClose() {
        setVisible(false);
    };
    function onNoClick(event) {
        onNo && onNo(event);
        onClose();
    }
    function onYesClick(event) {
        onYes && onYes(event);
        onClose();
    }

    return (
        <div>
            <Dialog
                open={visible ?? false}
                TransitionComponent={PopupTransition}
                keepMounted
                onClose={onClose}
                aria-describedby="alert-dialog-slide-description"
            >
                <DialogTitle>{t('common.confirmation')}</DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-slide-description">
                        {message}
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button onClick={onNoClick}>{t('common.no')}</Button>
                    <Button onClick={onYesClick}>{t('common.yes')}</Button>
                </DialogActions>
            </Dialog>
        </div>
    );
}