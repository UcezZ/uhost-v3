import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import PopupTransition from '../ui/PopupTransition';

export default function YesNoDialog({ visible, setVisible, onYes, onNo, message }) {
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
                open={visible}
                TransitionComponent={PopupTransition}
                keepMounted
                onClose={onClose}
                aria-describedby="alert-dialog-slide-description"
            >
                <DialogTitle>Подтверждение</DialogTitle>
                <DialogContent>
                    <DialogContentText id="alert-dialog-slide-description">
                        {message}
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button onClick={onNoClick}>Нет</Button>
                    <Button onClick={onYesClick}>Да</Button>
                </DialogActions>
            </Dialog>
        </div>
    );
}