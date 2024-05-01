import AuthForm from "../auth/AuthForm";
import Container from '@mui/material/Container';
import { useNavigate } from 'react-router-dom';
import config from '../../config.json';

export default function AuthPage() {
    const navigate = useNavigate();

    function onNext() {
        navigate(`${config.webroot}/`);
    }

    return (
        <Container sx={{ maxWidth: '800px !important' }}>
            <AuthForm next={onNext} />
        </Container>
    );
}