import { Box, Button, Typography } from '@mui/material';
import { useState } from 'react';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import PopupTransition from '../../ui/PopupTransition';
import AddIcon from '@mui/icons-material/Add';
import AddVideoForm from './AddVideoForm';
import { useTranslation } from 'react-i18next';

export default function AddVideoDialogButton() {
    const { t } = useTranslation();
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
        <Box sx={{
            display: 'flex',
            justifyContent: 'space-around',
            m: 2
        }}>
            <Button
                size='large'
                variant='contained'
                onClick={onClick}
                sx={{ gap: 1 }}>
                <AddIcon />
                <Typography variant='button'>{t('video.add')}</Typography>
            </Button>
            <Dialog
                open={visible ?? false}
                TransitionComponent={PopupTransition}
                keepMounted
                onClose={onClose}
                fullWidth>
                <DialogTitle>{t('video.add.caption')}</DialogTitle>
                <DialogContent >
                    <AddVideoForm next={onClose} setCanClose={setCanClose} />
                </DialogContent>
            </Dialog>
        </Box>
    );
}