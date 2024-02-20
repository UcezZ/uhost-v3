import { CircularProgress, Container } from "@mui/material";

export default function MessageBox({ text }) {
    return (
        <Container
            style={{
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'start',
                alignItems: 'center',
                height: '100vh',
            }}
        >
            <span style={{ marginTop: '16px' }}>{text}</span>
        </Container>
    )
}