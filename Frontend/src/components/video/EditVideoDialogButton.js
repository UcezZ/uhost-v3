import { Button, Typography, useMediaQuery } from '@mui/material';
import { useState, useTransition } from 'react';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import PopupTransition from '../../ui/PopupTransition';
import EditVideoForm from './EditVideoForm';
import EditIcon from '@mui/icons-material/Edit';
import { useTranslation } from 'react-i18next';

export default function EditVideoDialogButton({ video, setVideo }) {
    const { t } = useTranslation();
    const [visible, setVisible] = useState(false);
    const isNarrowScreen = useMediaQuery('(max-width:600px)');

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
                {!isNarrowScreen && <Typography variant='button'>{t('video.edit')}</Typography>}
            </Button>
            <Dialog
                open={visible ?? false}
                TransitionComponent={PopupTransition}
                keepMounted
                onClose={onClose}
            >
                <DialogTitle>{t('video.edit.caption')}</DialogTitle>
                <DialogContent>
                    <EditVideoForm video={video} setVideo={setVideo} next={onClose} />
                </DialogContent>
            </Dialog>
        </div>
    );

}