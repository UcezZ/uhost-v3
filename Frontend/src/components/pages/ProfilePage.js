import { Card, CardActions, CardContent, CardHeader, Container, Divider, Grid, TextField, Typography, useMediaQuery } from '@mui/material';
import { useContext, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useParams, } from 'react-router-dom';
import UserEndpoint from '../../api/UserEndpoint';
import Rights from '../../utils/Rights';
import StateContext from '../../utils/StateContext';
import Validation from '../../utils/Validation';
import CodeBlock from '../CodeBlock';
import LoadingBox from '../LoadingBox';
import ChangePasswordDialogButton from '../profile/ChangePasswordDialogButton';
import UpdateProfileDialogButton from '../profile/UpdateProfileDialogButton';
import NotFoundPage from './NotFoundPage';

export default function ProfilePage() {
    const { t } = useTranslation();
    const { login } = useParams();
    const { user } = useContext(StateContext);
    const [shownUser, setShownUser] = useState();
    const [loading, setLoading] = useState(true);
    const [isLoadingError, setIsLoadingError] = useState(false);
    const isNarrowScreen = useMediaQuery('(max-width:840px)');

    async function onUserLoad() {
        if (!isLoadingError && !shownUser && loading) {
            await UserEndpoint.getByLogin(login)
                .then(e => {
                    if (e?.data?.success && e?.data?.result) {
                        setShownUser(e.data.result);
                    } else {
                        setIsLoadingError(true);
                    }
                }).catch(e => {
                    setIsLoadingError(true);
                });
            setLoading(false);
        }
    }

    useEffect(() => {
        if (login?.length > 64 || login?.length < 3) {
            setIsLoadingError(true);
            return;
        }
        if (!shownUser && !isLoadingError) {
            if (user && (!login?.length || login?.toLowerCase() === user?.login?.toLowerCase())) {
                setShownUser(user);
                setLoading(false);
            } else {
                onUserLoad();
            }
        }
    }, [shownUser, user]);

    if (isLoadingError) {
        return (
            <NotFoundPage />
        );
    }

    if (loading) {
        return (
            <LoadingBox />
        );
    }

    const canModifyUser = shownUser?.id > 0 && user?.id > 0 && (shownUser.id === user.id || Rights.checkAnyRight(user, Rights.UserInteractAll));
    const canSeePersonalData = canModifyUser || Rights.checkAnyRight(Rights.UserCreate, Rights.UserDelete);

    return (
        <Container sx={{ maxWidth: '1152px !important' }}>
            <Grid container>
                <Grid item xs={isNarrowScreen ? 12 : 8}>
                    <Card>
                        <CardContent>
                            <Typography variant='h5'>
                                {t('user.profile.caption')}
                            </Typography>
                        </CardContent>
                        <Divider />
                        <CardContent>
                            <Grid container spacing={2} >
                                <Grid item xs={6} textAlign='right'>
                                    <Typography variant='h6'>
                                        {t('user.login')}:
                                    </Typography>
                                </Grid>
                                <Grid item xs={6} mt={0.5}>
                                    <Typography>
                                        {shownUser?.name ?? 'N/A'}
                                    </Typography>
                                </Grid>
                                <Grid item xs={6} textAlign='right'>
                                    <Typography variant='h6'>
                                        {t('user.name')}:
                                    </Typography>
                                </Grid>
                                <Grid item xs={6} mt={0.5}>
                                    <Typography>
                                        {shownUser?.name ?? 'N/A'}
                                    </Typography>
                                </Grid>
                                <Grid item xs={6} textAlign='right' >
                                    <Typography variant='h6'>
                                        {t('user.description')}:
                                    </Typography>
                                </Grid>
                                <Grid item xs={6} mt={0.5}>
                                    <Typography sx={{ whiteSpace: 'pre-wrap' }} >
                                        {shownUser?.description ?? 'N/A'}
                                    </Typography>
                                </Grid>
                                {
                                    canSeePersonalData && <Grid item xs={6} textAlign='right'>
                                        <Typography variant='h6'>
                                            {t('user.email')}:
                                        </Typography>
                                    </Grid>
                                }
                                {
                                    canSeePersonalData &&
                                    <Grid item xs={6} mt={0.5}>
                                        <Typography>
                                            {shownUser?.email ?? 'N/A'}
                                        </Typography>
                                    </Grid>
                                }
                                <Grid item xs={6} textAlign='right' alignItems='top'>
                                    <Typography variant='h6'>
                                        {t('user.lastlogin')}:
                                    </Typography>
                                </Grid>
                                <Grid item xs={6} mt={0.5}>
                                    <Typography>
                                        {shownUser?.lastVisitAt ?? 'N/A'}
                                    </Typography>
                                </Grid>
                                {
                                    canSeePersonalData && <Grid item xs={6} textAlign='right'>
                                        <Typography variant='h6'>
                                            {t('user.theme')}:
                                        </Typography>
                                    </Grid>
                                }
                                {
                                    canSeePersonalData &&
                                    <Grid item xs={6} mt={0.5}>
                                        <Typography>
                                            {shownUser?.theme?.length > 0 ? t(`user.theme.${shownUser.theme.toLowerCase()}`) : 'N/A'}
                                        </Typography>
                                    </Grid>
                                }
                                {
                                    canSeePersonalData && <Grid item xs={6} textAlign='right'>
                                        <Typography variant='h6'>
                                            {t('user.locale')}:
                                        </Typography>
                                    </Grid>
                                }
                                {
                                    canSeePersonalData &&
                                    <Grid item xs={6} mt={0.5}>
                                        <Typography>
                                            {shownUser?.locale?.length > 0 ? t(`user.locale.${shownUser.locale.toLowerCase()}`) : 'N/A'}
                                        </Typography>
                                    </Grid>
                                }
                                <Grid item xs={6} textAlign='right' alignItems='top'>
                                    <Typography variant='h6'>
                                        {t('user.videocount')}:
                                    </Typography>
                                </Grid>
                                <Grid item xs={6} mt={0.5}>
                                    <Typography>
                                        {shownUser?.videoCount ?? 'N/A'}
                                    </Typography>
                                </Grid>
                                <Grid item xs={6} textAlign='right' alignItems='top'>
                                    <Typography variant='h6'>
                                        {t('user.playlistcount')}:
                                    </Typography>
                                </Grid>
                                <Grid item xs={6} mt={0.5}>
                                    <Typography>
                                        {shownUser?.playlistCount ?? 'N/A'}
                                    </Typography>
                                </Grid>
                            </Grid>
                        </CardContent>
                        <CardActions>
                            {shownUser?.id > 0 && user?.id > 0 && shownUser.id === user.id && <ChangePasswordDialogButton />}
                            {canModifyUser && <UpdateProfileDialogButton id={shownUser.id} />}
                        </CardActions>
                    </Card>
                </Grid>
            </Grid>
            <CodeBlock data={shownUser} json />
        </Container>
    );
}