import { CircularProgress, Container } from "@mui/material";

const fullScreenStyle = {
    top: 0,
    left: 0,
    width: '100vw',
    height: '100vh'
}

export default function LoadingBox({ fullscreen }) {
    return (
        <Container
            style={{
                ...(fullscreen && fullScreenStyle),
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'center',
                alignItems: 'center',
                margin: fullscreen ? 0 : '2em'
            }}
        >
            <CircularProgress size={100} />
            <span style={{ marginTop: '16px' }}>Loading...</span>
        </Container>
    )
}