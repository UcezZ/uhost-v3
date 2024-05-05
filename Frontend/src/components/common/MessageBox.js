import { CircularProgress, Container } from "@mui/material";

export default function MessageBox({ text }) {
    return (
        <Container
            style={{
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'start',
                alignItems: 'center',
                height: '100%',
            }}
        >
            <span style={{ marginTop: '16px' }}>{text}</span>
        </Container>
    )
}