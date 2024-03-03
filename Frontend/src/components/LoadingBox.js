import { CircularProgress, Container } from "@mui/material";

export default function LoadingBox({ fullscreen }) {
    return (
        <Container
            style={{
                width: '100%',
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'center',
                alignItems: 'center',
                padding: '2em',
                margin: 'auto'
            }}
        >
            <CircularProgress size={100} />
            <span style={{ marginTop: '16px' }}>Loading...</span>
        </Container>
    )
}