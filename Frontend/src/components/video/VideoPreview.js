import { Avatar, Card, CardContent, CardHeader, CardMedia, Typography } from '@mui/material';
import { red } from '@mui/material/colors';
import { Box } from '@mui/system';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import config from '../../config.json';
import SafeImage from '../common/SafeImage';

export default function VideoPreview({ entity }) {
    const { t } = useTranslation();
    var login = entity?.user?.login?.length ? entity.user.login : 'N/A' ?? 'N/A';
    var avaText = login.at(0).toString().toUpperCase();
    var title = entity?.name ?? 'N/A';
    var createdAt = entity?.createdAt ?? 'N/A';
    var alias = entity?.token ?? 'N/A';

    return (
        <Card sx={{ width: '100%', maxWidth: '320px', margin: '16px' }}>
            <Link style={{ textDecoration: 'inherit', color: 'inherit' }} to={`${config.webroot}/profile/${login}`}>
                <CardHeader
                    avatar={
                        <Avatar sx={{ bgcolor: entity?.user?.avatarUrl?.length > 0 ? '#0000' : red[500] }} aria-label='recipe'>
                            {
                                entity?.user?.avatarUrl?.length > 0
                                    ? <SafeImage src={entity.user.avatarUrl} />
                                    : avaText
                            }
                        </Avatar>
                    }
                    title={login}
                    subheader={createdAt}
                />
            </Link>
            <Link style={{ textDecoration: 'inherit', color: 'inherit', position: 'relative' }} to={`${config.webroot}/video/${alias}`}>
                <CardMedia>
                    <SafeImage
                        src={entity?.thumbnailUrl}
                        height={180} />
                </CardMedia>
                <Typography variant='caption' sx={{
                    position: 'absolute',
                    bottom: 0,
                    right: 0,
                    background: '#0008',
                    color: 'white',
                    padding: '0px 4px',
                    margin: '4px',
                    borderRadius: '16px',
                    fontSize: '8pt'
                }}>
                    {entity?.duration}
                </Typography>
            </Link>
            <CardContent sx={{ flex: 1 }}>
                <Typography
                    variant='h6'
                    component='div'
                    sx={{
                        fontSize: 16,
                        fontWeight: 700
                    }}
                    noWrap>
                    {entity?.name ?? 'N/A'}
                </Typography>
                {
                    entity?.resolutions?.length > 0 &&
                    <Box sx={{ marginTop: '8px' }}>
                        {entity.resolutions.map((e, i) =>
                            <Typography variant='button' key={i} sx={{
                                bgcolor: red[500],
                                color: 'white',
                                padding: '4px',
                                margin: '4px',
                                borderRadius: '4px',
                                fontSize: '8pt',
                                fontWeight: '700'
                            }}>{t(`video.resolution.${e}`)}</Typography>
                        )}
                    </Box>
                }
                {/* {JSON.stringify(entity, null, 2)} */}

            </CardContent>
        </Card>
    )
}