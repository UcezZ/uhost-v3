import { Button, Card, CardContent, Typography } from '@mui/material';
import { Container } from '@mui/system';
import { red } from '@mui/material/colors';
import { Link } from 'react-router-dom';
import config from '../../config.json';
import { useTranslation } from 'react-i18next';
import SafeImage from '../common/SafeImage';

export default function NotFoundPage() {
    const { t } = useTranslation();

    return (
        <Container sx={{
            maxWidth: '100% !important',
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'center',
            alignItems: 'center',
            marginTop: 4
        }}>
            <Card sx={{
                width: '400px',
                maxWidth: '100%',
                textAlign: 'center'
            }}>
                <CardContent>
                    <SafeImage
                        src={`${config.webroot}/assets/idk.webp`}
                        width={256}
                        altElement={<Typography variant='h1' color={red[500]}>404</Typography>}
                    />
                    <Typography variant='content' component='p'>
                        {t('notfound.title')}
                    </Typography>
                    <Typography
                        variant='subtitle2'
                        sx={{ marginTop: 2 }}>
                        <i>{t('notfound.text')}</i>
                    </Typography>
                </CardContent>
                <CardContent>
                    <Link to={`${config.webroot}/`}>
                        <Button
                            fullWidth
                            variant='contained'
                            color='primary'
                            sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                        >
                            {t('notfound.returnhome')}
                        </Button>
                    </Link>
                </CardContent>
            </Card>
        </Container>
    );
}