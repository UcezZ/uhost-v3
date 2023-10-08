import AuthForm from "../forms/AuthForm";
import CssBaseline from '@mui/material/CssBaseline';
import Container from '@mui/material/Container';

export default function AuthPage() {
    return (
        <Container component="main" maxWidth="xs">
            <CssBaseline />
            <AuthForm />
        </Container>
    );
}