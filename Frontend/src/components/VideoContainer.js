import { Grid } from "@mui/material";

export default function VideoContainer({ videos }) {
    return (
        <div>
            <Grid container>
                <code><pre>{JSON.stringify(videos?.map(e => e.thumbnailUrl), null, 2)}</pre></code>
            </Grid>
        </div>
    );
}