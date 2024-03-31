import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import AuthForm from './AuthForm';
import PopupTransition from '../../ui/PopupTransition';
import { useTranslation } from 'react-i18next';

export default function PopupAuthForm({ visible, setVisible, next }) {
    const { t } = useTranslation();

    function onClose(event) {
        setVisible(false);
        next && next(event);
    };

    return (
        <div>
            <Dialog
                open={visible ?? false}
                TransitionComponent={PopupTransition}
                keepMounted
                onClose={onClose}
            >
                <DialogTitle>{t('auth.caption')}</DialogTitle>
                <DialogContent>
                    <AuthForm slim next={onClose} />
                </DialogContent>
            </Dialog>
        </div>
    );
}