import { CssBaseline } from "@mui/material";
import { Container } from "@mui/system";
import { useParams } from "react-router-dom";

export default function VideoPage() {
    const { token } = useParams();

    return (
        <Container sx={{ maxWidth: '100% !important' }}>
            <CssBaseline />
            {token}
        </Container>
    );
}