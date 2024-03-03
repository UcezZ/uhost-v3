import AuthForm from "../forms/AuthForm";
import Container from '@mui/material/Container';

export default function AuthPage() {
    return (
        <Container component="main" maxWidth="xs">
            <AuthForm />
        </Container>
    );
}