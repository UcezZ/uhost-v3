import { Button } from '@mui/base';
import { Card, CardActions, CardContent, Typography } from '@mui/material';
import { useContext } from 'react';
import { useTranslation } from 'react-i18next';
import Rights from '../../utils/Rights';
import StateContext from '../../utils/StateContext';
import DeleteRoleDialogButton from './DeleteRoleDialogButton';
import EditRoleDialogButton from './EditRoleDialogButton';

export default function RoleItem({ item, onUpdate }) {
    const { t } = useTranslation();
    const { user } = useContext(StateContext);

    return (
        <Card>
            <CardContent>
                <Typography><b>#{item?.id}</b>: {item?.name}</Typography>
                <div><b>{t('role.rightcount')}</b>: {item?.rightCount}</div>
                <div><b>{t('role.usercount')}</b>: {item?.userCount}</div>
                <div><b>{t('role.createdat')}</b>: {item?.createdAt}</div>
            </CardContent>
            <CardActions>
                {Rights.checkAnyRight(user, Rights.RoleCreateUpdate) && <EditRoleDialogButton role={item} next={onUpdate} />}
                {item.id > 1 && Rights.checkAnyRight(user, Rights.RoleDelete) && <DeleteRoleDialogButton role={item} next={onUpdate} />}
            </CardActions>
        </Card>
    )
}