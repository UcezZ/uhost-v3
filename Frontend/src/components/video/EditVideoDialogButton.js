import { Button, Typography } from '@mui/material';
import { useState } from 'react';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import PopupTransition from '../../ui/PopupTransition';
import EditVideoForm from './EditVideoForm';
import EditIcon from '@mui/icons-material/Edit';

export default function EditVideoDialogButton({ video, setVideo }) {
    const [visible, setVisible] = useState(false);

    function onClick(event) {
        setVisible(true);
    };
    function onClose() {
        setVisible(false);
    };

    return (
        <div>
            <Button
                size='large'
                color='inherit'
                variant='contained'
                onClick={onClick}
                sx={{ gap: 1 }}>
                <EditIcon />
                <Typography variant='button'>
                    Редактировать...
                </Typography>
            </Button>
            <Dialog
                open={visible ?? false}
                TransitionComponent={PopupTransition}
                keepMounted
                onClose={onClose}
            >
                <DialogTitle>Редактирование видео</DialogTitle>
                <DialogContent>
                    <EditVideoForm video={video} setVideo={setVideo} next={onClose} />
                </DialogContent>
            </Dialog>
        </div>
    );

}