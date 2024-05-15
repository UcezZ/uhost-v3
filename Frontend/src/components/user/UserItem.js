import { Avatar, Card, CardActions, CardContent, CardHeader, Typography } from '@mui/material';
import { useContext } from 'react';
import { useTranslation } from 'react-i18next';
import Rights from '../../utils/Rights';
import StateContext from '../../utils/StateContext';
import UserUpdateDialogButton from './UserUpdateDialogButton';
import SafeImage from '../common/SafeImage';
import { red } from '@mui/material/colors';

export default function UserItem({ item, onUpdate }) {
    const { t } = useTranslation();
    const { user } = useContext(StateContext);
    const login = item?.login?.length > 0 ? item.login : 'N/A';
    const avaText = login.at(0).toUpperCase();

    return (
        <Card>
            <CardHeader
                avatar={
                    <Avatar sx={{ bgcolor: item?.avatarUrl?.length > 0 ? '#0000' : red[500] }} >
                        {
                            item?.avatarUrl?.length > 0
                                ? <SafeImage src={item.avatarUrl} />
                                : avaText
                        }
                    </Avatar>
                }
                title={login}
                subheader={`#${item?.id}`}
            />
            <CardContent>
                <div><b>{t('user.name')}</b>: {item?.name}</div>
                <div><b>{t('user.email')}</b>: {item?.email}</div>
                <div><b>{t('user.roles')}</b>: {item?.roleIds?.length ?? 0}</div>
                <div><b>{t('user.createdat')}</b>: {item?.createdAt}</div>
                <div><b>{t('user.lastlogin')}</b>: {item?.lastVisitAt}</div>
                <div><b>{t('user.locale')}</b>: {t(`user.locale.${item?.locale?.toLowerCase()}`)}</div>
                <div><b>{t('user.theme')}</b>: {t(`user.theme.${item?.theme?.toLowerCase()}`)}</div>
            </CardContent>
            <CardActions>
                {Rights.checkAnyRight(user, Rights.UserCreate) && <UserUpdateDialogButton user={item} next={onUpdate} />}
            </CardActions>
        </Card>
    )
}