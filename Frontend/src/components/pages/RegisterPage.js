import { Container, Typography } from '@mui/material';
import { useState } from 'react';
import RegisterCompleted from '../register/RegisterCompleted';
import RegisterConfirmForm from '../register/RegisterConfirmForm';
import RegisterForm from '../register/RegisterForm';
import { useTranslation } from 'react-i18next';

export default function RegisterPage() {
    const [stage, setStage] = useState(0);
    const { t } = useTranslation();

    function getForm() {
        switch (stage) {
            case 0:
                return (
                    <RegisterForm next={e => setStage(stage + 1)} prev={e => history.back()} />
                );
            case 1:
                return (
                    <RegisterConfirmForm next={e => setStage(stage + 1)} prev={e => setStage(stage - 1)} />
                );
            case 2:
                return (
                    <RegisterCompleted />
                );
            default:
                setStage(0);
        }
    }

    return (
        <Container sx={{ maxWidth: '800px !important' }}>
            <Typography variant='h4' mt={2} mb={2}>{t('register.caption')}</Typography>
            {getForm()}
        </Container>
    );
}