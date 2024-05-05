import { Button, Dialog, DialogContent, DialogTitle, Typography } from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import { useTranslation } from 'react-i18next';
import { useState, useEffect } from 'react';
import PopupTransition from '../../ui/PopupTransition';
import UserUpdateForm from './UserUpdateForm';
import LoadingBox from '../common/LoadingBox';
import RoleEndpoint from '../../api/RoleEndpoint';

export default function UserUpdateDialogButton({ user, next }) {
    const { t } = useTranslation();
    const [visible, setVisible] = useState(false);
    const [loading, setLoading] = useState(true);
    const [allRoles, setAllRoles] = useState([]);

    async function onLoad() {
        if (!loading || !visible) {
            return;
        }

        await RoleEndpoint.getAll(1, Math.pow(2 ^ 31) - 1, 'id')
            .then(e => {
                if (e?.data?.success && e?.data?.result?.items) {
                    setAllRoles(e.data.result.items);
                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));

        setLoading(false);
    }

    function onClick() {
        setVisible(true);
    };
    function onClose() {
        setVisible(false);
    };
    function onNext() {
        next && next();
        onClose();
    }

    useEffect(() => {
        onLoad();
    }, [loading, visible]);

    return (
        <div style={{ flex: 1 }}>
            <Button
                size='large'
                variant='contained'
                onClick={onClick}
                fullWidth
                sx={{ gap: 1 }}>
                <EditIcon />
                <Typography variant='button'>{t('user.edit')}</Typography>
            </Button>
            <Dialog
                open={visible ?? false}
                TransitionComponent={PopupTransition}
                keepMounted
                onClose={onClose}
                fullWidth>
                <DialogTitle>{t('user.edit.caption')}</DialogTitle>
                <DialogContent >
                    {
                        loading
                            ? <LoadingBox />
                            : <UserUpdateForm user={user} next={onNext} onClose={onClose} allRoles={allRoles} />
                    }
                </DialogContent>
            </Dialog>
        </div>
    )
}