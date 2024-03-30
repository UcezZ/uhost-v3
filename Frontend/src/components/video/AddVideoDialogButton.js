import { Button, Typography } from '@mui/material';
import { useState } from 'react';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import PopupTransition from '../../ui/PopupTransition';
import AddIcon from '@mui/icons-material/Add';
import AddVideoForm from './AddVideoForm';

export default function AddVideoDialogButton() {
    const [visible, setVisible] = useState(false);
    const [canClose, setCanClose] = useState(true);

    function onClick() {
        setVisible(true);
    };
    function onClose() {
        if (canClose) {
            setVisible(false);
        }
    };

    return (
        <div>
            <Button
                size='large'
                variant='contained'
                onClick={onClick}
                sx={{ gap: 1 }}>
                <AddIcon />
                <Typography variant='button'>
                    Добавить видео
                </Typography>
            </Button>
            <Dialog
                open={visible ?? false}
                TransitionComponent={PopupTransition}
                keepMounted
                onClose={onClose}
                fullWidth>
                <DialogTitle>Добавление видео</DialogTitle>
                <DialogContent >
                    <AddVideoForm next={onClose} setCanClose={setCanClose} />
                </DialogContent>
            </Dialog>
        </div>
    );
}