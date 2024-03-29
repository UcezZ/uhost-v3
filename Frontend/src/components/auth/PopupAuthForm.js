import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import AuthForm from './AuthForm';
import PopupTransition from '../../ui/PopupTransition';

export default function PopupAuthForm({ visible, setVisible, next }) {
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
                <DialogTitle>Авторизация</DialogTitle>
                <DialogContent>
                    <AuthForm slim next={onClose} />
                </DialogContent>
            </Dialog>
        </div>
    );
}