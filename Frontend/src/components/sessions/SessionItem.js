import { Avatar, Button, Card, CardActions, CardHeader, CircularProgress, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import Image from '../common/Image';
import { useContext, useState } from 'react';
import YesNoDialog from '../common/YesNoDialog';
import SessionEndpoint from '../../api/SessionEndpoint';
import Common from '../../utils/Common';
import StateContext from '../../utils/StateContext';
import { red } from '@mui/material/colors';

export default function SessionItem({ item, onTerminated }) {
    const [loading, setLoading] = useState(false);
    const [dialogVisible, setDialogVisible] = useState(false);
    const { t } = useTranslation();
    const { setUser, setError } = useContext(StateContext);
    var login = item?.user?.login?.length ? item.user.login : 'N/A' ?? 'N/A';
    var avaText = login.at(0).toString().toUpperCase();

    async function onTerminateConfirm() {
        setLoading(true);

        await SessionEndpoint.terminate(item?.sessionGuid)
            .then(e => {
                if (e?.data?.success && e?.data?.result) {
                    onTerminated && onTerminated();
                    if (item?.isCurrent === true) {
                        setUser();
                        Common.resetToken();
                    }
                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));

        setLoading(false);
    }

    function onTerminateClick() {
        if (item?.isCurrent === true) {
            setDialogVisible(true);
        } else {
            onTerminateConfirm();
        }
    }

    return (
        <Card sx={{ m: 1 }}>
            <CardHeader
                avatar={
                    <Avatar sx={{ bgcolor: item?.user?.avatarUrl?.length > 0 ? '#0000' : red[500] }} aria-label='recipe'>
                        {
                            item?.user?.avatarUrl?.length > 0
                                ? <Image src={item.user.avatarUrl} />
                                : avaText
                        }
                    </Avatar>
                }
                title={login}
                subheader={`${t('user.lastlogin')}: ${item?.user?.lastVisitAt ?? 'never'}`}
            />
            <div style={{ padding: '0 .5em' }} >
                <div><b>{t('session.guid')}:</b> {item?.sessionGuid}</div>
                <div><b>{t('session.expiresat')}:</b> {item?.expiresAt}</div>
                <div><b>{t('session.expiresin')}:</b> {item?.expiresIn}</div>
                {item?.clientInfo?.ipAddress?.length && <div><b>{t('session.ip')}</b>: {item?.clientInfo?.ipAddress}</div>}
                {item?.clientInfo?.client?.length && <div><b>{t('session.client')}</b>: {item?.clientInfo?.client}</div>}
                <div><b>{t('session.iscurrent')}:</b> {item?.isCurrent === true ? t('common.yes') : t('common.no')}</div>
            </div>
            <CardActions>
                <Button
                    size='large'
                    color='primary'
                    variant='contained'
                    disabled={loading}
                    fullWidth
                    onClick={onTerminateClick}>
                    {loading ? <CircularProgress size={20} /> : t('session.terminate')}
                </Button>
            </CardActions>
            <YesNoDialog visible={dialogVisible} setVisible={setDialogVisible} onYes={onTerminateConfirm} message={t('session.terminate.self.confirm')} />
        </Card>
    );
}