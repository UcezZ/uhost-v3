import { useContext } from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import StateContext from '../../utils/StateContext';
import PopupTransition from '../../ui/PopupTransition';
import { useTranslation } from 'react-i18next';

var errContent;

export default function ErrorDialog({ message }) {
    const { t } = useTranslation();
    const { error, setError } = message
        ? { error: message, setError: () => { } }
        : useContext(StateContext);

    const handleClose = () => {
        setError(null);
    };

    if (error) {
        errContent = error;
    }

    return (
        <Dialog
            open={error != null}
            TransitionComponent={PopupTransition}
            keepMounted
            onClose={handleClose}
            sx={{ zIndex: 1333 }}
        >
            <DialogTitle>{t('common.error')}</DialogTitle>
            <DialogContent color='red'>
                {
                    errContent?.map
                        ? errContent.map((e, i) => <DialogContentText key={i}>{e}</DialogContentText>)
                        : <DialogContentText>{errContent}</DialogContentText>
                }
            </DialogContent>
            <DialogActions>
                <Button onClick={handleClose}>{t('common.close')}</Button>
            </DialogActions>
        </Dialog>
    );
}