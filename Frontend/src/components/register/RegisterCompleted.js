import React, { useContext, useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Box from '@mui/material/Box';
import StateContext from '../../utils/StateContext';
import { CircularProgress, FormControl, Grid, InputLabel, MenuItem, Select, Typography } from '@mui/material';
import Common from '../../utils/Common';
import Validation from '../../utils/Validation';
import Styles from '../../ui/Styles';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import config from '../../config.json';

export default function RegisterCompleted() {
    const { t } = useTranslation();
    const navigate = useNavigate();

    function onNext() {
        navigate(`${config.webroot}/login`);
    }

    return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
            }}
        >
            <Typography textAlign='justify' width='100%' >{t('register.completed')}</Typography>
            <Box sx={{
                display: 'flex',
                flexDirection: 'row',
                alignItems: 'center',
                gap: 2,
                ...Styles.noSelectSx,
                width: '100%'
            }}>
                <Button
                    fullWidth
                    variant='contained'
                    sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                    onClick={onNext}
                >
                    {t('common.next')}
                </Button>
            </Box>
        </Box>
    );
}