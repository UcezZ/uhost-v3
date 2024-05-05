import { Card, CardActions, CardContent, Container, Divider, Grid, Typography, useMediaQuery } from '@mui/material';
import { useContext, useEffect, useState } from 'react';
import { useParams, } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import ChangePasswordDialogButton from '../profile/ChangePasswordDialogButton';
import DeleteAvatarDialogButton from '../profile/DeleteAvatarDialogButton';
import Image from '../common/Image';
import LoadingBox from '../common/LoadingBox';
import NotFoundPage from './NotFoundPage';
import Rights from '../../utils/Rights';
import StateContext from '../../utils/StateContext';
import UpdateProfileDialogButton from '../profile/UpdateProfileDialogButton';
import UploadAvatarDialogButton from '../profile/UploadAvatarDialogButton';
import UserEndpoint from '../../api/UserEndpoint';

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
    const isCurrentUser = shownUser?.id > 0 && user?.id > 0 && shownUser.id === user.id;

    return (
        <Container sx={{ maxWidth: '1280px !important' }}>
            <Grid container mt={2} columnSpacing={2}>
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
                                        {shownUser?.login ?? 'N/A'}
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
                        {(isCurrentUser || canModifyUser) && <Divider />}
                        {
                            (isCurrentUser || canModifyUser) && <CardActions>
                                {isCurrentUser && <ChangePasswordDialogButton />}
                                {canModifyUser && <UpdateProfileDialogButton shownUser={shownUser} />}
                            </CardActions>
                        }
                    </Card>
                </Grid>

                <Grid item xs={isNarrowScreen ? 12 : 4}>
                    <Card>
                        <CardContent>
                            <Typography variant='h5'>
                                {t('user.profile.avatar')}
                            </Typography>
                        </CardContent>
                        <Divider />
                        <CardContent>
                            <Image
                                src={shownUser?.avatarUrl}
                                height={400}
                            />
                        </CardContent>
                        {isCurrentUser && <Divider />}
                        {
                            isCurrentUser &&
                            <CardActions>
                                <UploadAvatarDialogButton shownUser={shownUser} setShownUser={setShownUser} />
                                {shownUser.avatarUrl?.length > 0 && <DeleteAvatarDialogButton shownUser={shownUser} setShownUser={setShownUser} />}
                            </CardActions>
                        }
                    </Card>
                </Grid>
            </Grid>
        </Container>
    );
}