import { Box, Typography } from '@mui/material';
import { useContext } from 'react';
import { useTranslation } from 'react-i18next';
import StateContext from '../../utils/StateContext';

export default function VideoDummy({ video }) {
    const { t } = useTranslation();
    const { user } = useContext(StateContext);

    return (
        <Box
            component='div'
            style={{
                flex: 1,
                padding: 0,
                margin: 0,
                justifyContent: 'center',
                maxWidth: '100%',
                display: 'flex',
                height: '100%',
                minHeight: '300px',
                backgroundImage: `url('${video?.thumbnailUrl}')`,
                backgroundRepeat: 'no-repeat',
                backgroundSize: 'contain',
                backgroundPosition: 'center center'
            }} >
            <Typography
                component='div'
                sx={{
                    textAlign: 'center',
                    flex: 1,
                    m: 12,
                    p: 2,
                    color: '#eee',
                    borderRadius: 2,
                    display: 'flex',
                    flexDirection: 'column',
                    gap: 1,
                    backgroundColor: '#000A',
                    maxWidth: 400
                }}
            >
                <span>{t(video?.resolutions?.length ? 'video.error.notsupported' : 'video.error.notprocessed')}</span>
                {user?.id > 0 && user?.id === video?.userId && <span>{t('video.error.checkprocessing')}</span>}
            </Typography>
        </Box>
    )
}