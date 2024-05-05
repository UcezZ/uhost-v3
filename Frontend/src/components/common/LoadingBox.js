import { CircularProgress, Container } from "@mui/material";
import { useTranslation } from 'react-i18next';

export default function LoadingBox() {
    const { t } = useTranslation();

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
            <span style={{ marginTop: '16px' }}>{t('common.loading')}</span>
        </Container>
    )
}