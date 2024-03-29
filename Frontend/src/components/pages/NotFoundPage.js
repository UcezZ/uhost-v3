import { Button, Card, CardContent, Typography } from '@mui/material';
import { Container } from '@mui/system';
import { red } from '@mui/material/colors';
import { Link } from 'react-router-dom';
import config from '../../config.json';

export default function NotFoundPage() {
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
                    <Typography variant='h1' color={red[500]}>
                        404
                    </Typography>
                    <Typography variant='content' component='p'>
                        Запрашиваемая страница не найдена
                    </Typography>
                    <Typography
                        variant='subtitle2'
                        sx={{ marginTop: 2 }}>
                        <i>...или ещё не реализована</i>
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
                            Вернуться на главную
                        </Button>
                    </Link>
                </CardContent>
            </Card>
        </Container>
    );
}