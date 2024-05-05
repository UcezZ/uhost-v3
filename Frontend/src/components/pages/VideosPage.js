import { Box, Container, Typography } from '@mui/material';
import { useContext, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useParams, useSearchParams } from 'react-router-dom';
import VideoEndpoint from '../../api/VideoEndpoint';
import Common from '../../utils/Common';
import StateContext from '../../utils/StateContext';
import SearchBar from '../common/SearchBar';
import AddVideoDialogButton from '../video/AddVideoDialogButton';
import VideoSearchResult from '../video/VideoSearchResult';
import NotFoundPage from './NotFoundPage';

export default function Videos() {
    const { t } = useTranslation();
    const { user } = useContext(StateContext);
    const [search, setSearch] = useState('');
    const { login } = useParams()
    const targetLogin = login ?? user.login;
    const isSameUser = targetLogin === user.login;

    function onSearch(value) {
        if (value?.toLowerCase() !== search?.toLocaleLowerCase()) {
            setSearch(value ?? '');
        }
    }

    if (!targetLogin?.length) {
        return (
            <NotFoundPage />
        );
    }

    return (
        <Container sx={{ maxWidth: '100% !important' }}>
            <SearchBar sx={{ marginTop: 1 }} onSearch={onSearch} />
            {
                user?.id > 0 && <AddVideoDialogButton />
            }
            <Typography variant='h4' m={2}>{isSameUser ? t('video.my') : t('video.ofuser', { user: targetLogin })}</Typography>
            <VideoSearchResult
                userLogin={targetLogin}
                query={search}
                sortBy='CreatedAt'
                sortDir={search?.length && 'Desc'}
                showHidden
                showPrivate
                usePager />
        </Container>
    );
}