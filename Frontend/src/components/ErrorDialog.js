import { forwardRef, useContext } from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import StateContext from '../utils/StateContext';
import PopupTransition from '../ui/PopupTransition';
import { useTranslation } from 'react-i18next';

var errContent;

export default function ErrorDialog() {
    const { t } = useTranslation();
    const { error, setError } = useContext(StateContext);

    const handleClose = () => {
        setError(null);
    };

    if (error) {
        errContent = error;
    }

    return (
        <div>
            <Dialog
                open={error != null}
                TransitionComponent={PopupTransition}
                keepMounted
                onClose={handleClose}
                aria-describedby="alert-dialog-slide-description"
            >
                <DialogTitle>{t('common.error')}</DialogTitle>
                <DialogContent color='red'>
                    {/* <DialogContentText id="alert-dialog-slide-description"> */}
                    {
                        errContent?.map
                            ? errContent.map((e, i) => <DialogContentText key={i}>{e}</DialogContentText>)
                            : <DialogContentText>{errContent}</DialogContentText>
                    }
                    {/* </DialogContentText> */}
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleClose}>{t('common.close')}</Button>
                </DialogActions>
            </Dialog>
        </div>
    );
}