import React, { Component } from 'react';
import { Button, Card, CardContent, Typography } from '@mui/material';
import { Container } from '@mui/system';
import config from '../../config.json';
import Common from '../../utils/Common';
import * as Sentry from '@sentry/browser';

export default class ErrorBoundary extends Component {
    constructor(props) {
        super(props);
        this.state = { hasError: false };
    }

    static getDerivedStateFromError(error) {
        Sentry.captureException(error);
        return { hasError: true };
    }

    render() {
        if (this.state.hasError) {
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
                            <img
                                src={`${config.webroot}/assets/drunk.webp`}
                                style={{
                                    width: '256px',
                                    height: '256px',
                                    objectFit: 'cover',
                                    margin: '1em'
                                }}
                            />
                            <Typography variant='content' component='p'>
                                {Common.i18n('error.title')}
                            </Typography>
                            <Typography
                                variant='body2'
                                sx={{ marginTop: 2 }}>
                                {Common.i18n('error.text1')}
                            </Typography>
                            <Typography
                                variant='subtitle2'
                                sx={{ marginTop: 2 }}>
                                <i>{Common.i18n('error.text2')}</i>
                            </Typography>
                        </CardContent>
                        <CardContent>
                            <a href={`${config.webroot}/`}>
                                <Button
                                    fullWidth
                                    variant='contained'
                                    color='primary'
                                    sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                                >
                                    {Common.i18n('notfound.returnhome')}
                                </Button>
                            </a>
                        </CardContent>
                    </Card>
                </Container>
            );
        }

        return this.props.children;
    }
}