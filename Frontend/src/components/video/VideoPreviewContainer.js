import { Grid } from '@mui/material';
import VideoPreview from './VideoPreview';

export default function VideoPreviewContainer({ videos }) {
    return (
        <Grid container sx={{ justifyContent: 'space-around' }}>
            {videos?.map && videos.map((e, i) => <VideoPreview entity={e} key={i} />)}
        </Grid>
    );
}