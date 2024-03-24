import AuthForm from "../auth/AuthForm";
import Container from '@mui/material/Container';

export default function AuthPage() {
    return (
        <Container component="main" maxWidth="xs">
            <AuthForm />
        </Container>
    );
}