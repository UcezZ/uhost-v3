import { Grid } from "@mui/material";
import VideoPreview from "./items/VideoPreview";

export default function VideoContainer({ videos }) {
    return (
        <Grid container sx={{ justifyContent: 'space-around' }}>
            {videos?.map && videos.map(e => <VideoPreview entity={e} />)}
        </Grid>
    );
}