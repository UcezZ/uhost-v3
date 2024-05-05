import { Button } from '@mui/material';
import { useState, useContext } from 'react';
import DeleteIcon from '@mui/icons-material/Delete';
import YesNoDialog from '../common/YesNoDialog';
import StateContext from '../../utils/StateContext';
import { useTranslation } from 'react-i18next';
import UserEndpoint from '../../api/UserEndpoint';
import Common from '../../utils/Common';

export default function DeleteAvatarDialogButton({ shownUser, setShownUser }) {
    const { t } = useTranslation();
    const { setError, user, setUser } = useContext(StateContext);
    const [visible, setVisible] = useState(false);

    async function onYes() {
        await UserEndpoint.deleteAvatarSelf()
            .then(e => {
                if (e?.data?.success) {
                    user && setUser && setUser({ ...user, avatarUrl: null });
                    shownUser && setShownUser && setShownUser({ ...shownUser, avatarUrl: null });
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
        <div>
            <Button
                size='large'
                variant='contained'
                onClick={e => setVisible(true)}
                color='primary'
                sx={{
                    pl: 0,
                    pr: 0
                }}
            >
                <DeleteIcon />
            </Button>
            <YesNoDialog
                visible={visible}
                setVisible={setVisible}
                message={t('user.profile.avatar.delete.confirm')}
                onYes={onYes}
                onNo={onNo} />
        </div>
    );

}