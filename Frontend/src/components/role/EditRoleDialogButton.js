import { Button, Typography, useMediaQuery } from '@mui/material';
import { useState, useEffect, useContext } from 'react';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import PopupTransition from '../../ui/PopupTransition';
import EditRoleForm from './EditRoleForm';
import EditIcon from '@mui/icons-material/Edit';
import { useTranslation } from 'react-i18next';
import Common from '../../utils/Common';
import RoleEndpoint from '../../api/RoleEndpoint';
import LoadingBox from '../common/LoadingBox';
import StateContext from '../../utils/StateContext';

export default function EditRoleDialogButton({ role, next }) {
    const { t } = useTranslation();
    const [visible, setVisible] = useState(false);
    const [allRights, setAllRights] = useState([]);
    const [rightLoading, setRightLoading] = useState(false);
    const { setError } = useContext(StateContext);
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
        <div style={{ width: '100%' }}>
            <Button
                size='large'
                color='inherit'
                variant='contained'
                disabled={rightLoading}
                onClick={onClick}
                fullWidth
                sx={{ gap: 1, minWidth: '160px' }}>
                <EditIcon />
                {!isNarrowScreen && <Typography variant='button'>{t('role.edit')}</Typography>}
            </Button>
            <Dialog
                open={visible ?? false}
                TransitionComponent={PopupTransition}
                keepMounted
                onClose={onClose}
            >
                <DialogTitle>{t('role.edit.caption')}</DialogTitle>
                <DialogContent>
                    {
                        rightLoading
                            ? <LoadingBox />
                            : <EditRoleForm role={role} allRights={allRights} next={onNext} onClose={onClose} />
                    }
                </DialogContent>
            </Dialog>
        </div>
    );

}