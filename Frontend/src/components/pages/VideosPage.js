import { Box, Container } from '@mui/material';
import { useContext, useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
import VideoEndpoint from '../../api/VideoEndpoint';
import Common from '../../utils/Common';
import StateContext from '../../utils/StateContext';
import SearchBar from '../items/SearchBar';
import AddVideoDialogButton from '../video/AddVideoDialogButton';
import VideoSearchResult from '../video/VideoSearchResult';
import NotFoundPage from './NotFoundPage';

export default function Videos() {
    const { user } = useContext(StateContext);
    const [search, setSearch] = useState('');
    const [searchParams] = useSearchParams();
    const login = searchParams?.get('u') ?? user.login;

    function onSearch(value) {
        if (value?.toLowerCase() !== search?.toLocaleLowerCase()) {
            setSearch(value ?? '');
        }
    }

    if (!login?.length) {
        return (
            <NotFoundPage />
        );
    }

    return (
        <Container sx={{ maxWidth: '100% !important' }}>
            <Container sx={{ maxWidth: '1152px !important' }}>
                <SearchBar sx={{ marginTop: 1 }} onSearch={onSearch} />
            </Container>
            {
                user?.id > 0 && <Box sx={{
                    display: 'flex',
                    justifyContent: 'space-around',
                    m: 2
                }}>
                    <AddVideoDialogButton />
                </Box>
            }
            <VideoSearchResult
                userLogin={login}
                query={search}
                sortBy='CreatedAt'
                sortDir={search?.length && 'Desc'}
                usePager />
        </Container>
    );
}