import { Box, Button, Typography, useMediaQuery } from '@mui/material';
import { useState, useEffect } from 'react';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import PopupTransition from '../../ui/PopupTransition';
import AddRoleForm from './AddRoleForm';
import AddIcon from '@mui/icons-material/Add';
import { useTranslation } from 'react-i18next';
import Common from '../../utils/Common';
import RoleEndpoint from '../../api/RoleEndpoint';
import LoadingBox from '../common/LoadingBox';

export default function AddRoleDialogButton({ next }) {
    const { t } = useTranslation();
    const [visible, setVisible] = useState(false);
    const [allRights, setAllRights] = useState([]);
    const [rightLoading, setRightLoading] = useState(false);
    const isNarrowScreen = useMediaQuery('(max-width:600px)');

    async function onRightLoad() {
        if (!rightLoading || !visible) {
            return;
        }

        await RoleEndpoint.rights()
            .then(e => {
                if (e?.data?.success && e?.data?.result) {
                    setAllRights(e.data.result);
                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));

        setRightLoading(false);
    }

    function onClick() {
        setVisible(true);
        setRightLoading(true);
    };
    async function onNext() {
        onClose();
        await Common.sleep(300);
        next && next();
    }
    function onClose() {
        setVisible(false);
    };

    useEffect(() => {
        onRightLoad();
    }, [rightLoading, visible]);

    return (
        <Box sx={{
            display: 'flex',
            justifyContent: 'space-around',
            m: 2
        }}>
            <Button
                size='large'
                color='primary'
                variant='contained'
                disabled={rightLoading}
                onClick={onClick}
                sx={{ gap: 1 }}>
                <AddIcon />
                {!isNarrowScreen && <Typography variant='button'>{t('role.add')}</Typography>}
            </Button>
            <Dialog
                open={visible ?? false}
                TransitionComponent={PopupTransition}
                keepMounted
                onClose={onClose}
            >
                <DialogTitle>{t('role.add.caption')}</DialogTitle>
                <DialogContent>
                    {
                        rightLoading
                            ? <LoadingBox />
                            : <AddRoleForm allRights={allRights} next={onNext} onClose={onClose} />
                    }
                </DialogContent>
            </Dialog>
        </Box>
    );

}