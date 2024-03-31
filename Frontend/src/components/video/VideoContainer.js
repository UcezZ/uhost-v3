import { Avatar, Card, CardActions, CardHeader, Typography, CardContent } from '@mui/material';
import { useState, useContext } from 'react';
import { Link } from 'react-router-dom';
import { red } from '@mui/material/colors';
import DownloadButton from './DownloadButton';
import StateContext from '../../utils/StateContext';
import Rights from '../../utils/Rights';
import EditVideoDialogButton from './EditVideoDialogButton';
import DeleteVideoDialogButton from './DeleteVideoDialogButton';
import VideoPlayer from './VideoPlayer';
import config from '../../config.json';
import { useTranslation } from 'react-i18next';

export default function VideoContainer({ video, setVideo }) {
    const { t } = useTranslation();
    const { user } = useContext(StateContext);
    const sizes = video?.resolutions?.map
        ? video.resolutions
            .filter(e => `video${e}` in video?.downloadSizes)
            .map(e => {
                return {
                    key: e,
                    size: video.downloadSizes[`video${e}`]
                }
            })
        : [];
    const canEditVideo = user?.id && (video?.id && user.id === video.id || Rights.checkAnyRight(user, Rights.VideoGetAll));
    var login = video?.user?.login ?? 'N/A';
    var avaText = login.at(0).toString().toUpperCase();

    const [largeMode, setLargeMode] = useState(false);

    function toggleLargeMode() {
        setLargeMode && setLargeMode(!largeMode);
    }

    return (
        <Card sx={{ marginTop: 3 }}>
            <Link style={{ textDecoration: 'inherit', color: 'inherit' }} to={`${config.webroot}/profile/${login}`}>
                <CardHeader
                    avatar={
                        <Avatar sx={{ bgcolor: red[500] }} aria-label='recipe'>
                            {
                                video?.user?.avatarUrl?.length > 0
                                    ? <img src={entity.avatarUrl} alt={avaText} />
                                    : avaText
                            }
                        </Avatar>
                    }
                    title={login}
                    subheader={video?.createdAt}
                />
            </Link>
            <VideoPlayer video={video} largeMode={largeMode} />
            <CardActions>
                <DownloadButton token={video?.token} sizes={sizes} />
                {canEditVideo && <EditVideoDialogButton video={video} setVideo={setVideo} />}
                {canEditVideo && <DeleteVideoDialogButton video={video} setVideo={setVideo} />}
            </CardActions>
            {
                video?.description?.length > 0 && <CardContent>
                    <Typography variant='h6'>{t('video.description')}:</Typography>
                    <Typography
                        variant='body1'
                        sx={{ whiteSpace: 'pre-line' }} >
                        {video.description}
                    </Typography>
                </CardContent>
            }
        </Card>
    );
}