import { Button, Typography, useMediaQuery } from '@mui/material';
import { useState, useContext } from 'react';
import DeleteIcon from '@mui/icons-material/Delete';
import YesNoDialog from '../common/YesNoDialog';
import RoleEndpoint from '../../api/RoleEndpoint';
import StateContext from '../../utils/StateContext';
import { useTranslation } from 'react-i18next';

export default function DeleteRoleDialogButton({ role, next }) {
    const { t } = useTranslation();
    const { setError } = useContext(StateContext);
    const [visible, setVisible] = useState(false);
    const isNarrowScreen = useMediaQuery('(max-width:600px)');

    async function onYes() {
        await RoleEndpoint.delete(role.id)
            .then(e => {
                if (e?.data?.success && e?.data?.result) {
                    next && next();
                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));

        setVisible(false);
    };

    function onNo() {
        setVisible(false);
    };

    return (
        <div style={{ width: '100%' }}>
            <Button
                size='large'
                variant='contained'
                onClick={e => setVisible(true)}
                color='primary'
                fullWidth
                sx={{ gap: 1 }}>
                <DeleteIcon />
                {!isNarrowScreen && <Typography variant='button'>{t('role.delete')}</Typography>}
            </Button>
            <YesNoDialog
                visible={visible}
                setVisible={setVisible}
                message={t('role.delete.confirm', { name: role?.name ?? 'N/A' })}
                onYes={onYes}
                onNo={onNo} />
        </div>
    );

}