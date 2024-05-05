import { Container, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';

export default function NoDataBox() {
    const { t } = useTranslation();

    return (
        <Container sx={{ textAlign: 'center', width: '100% !important' }}>
            <Typography variant='body1' m={3}>{t('common.nodata')}</Typography>
        </Container>
    );
}