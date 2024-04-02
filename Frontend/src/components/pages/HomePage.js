import { Container } from '@mui/system';
import { useContext, useEffect, useState } from 'react';
import SearchBar from '../items/SearchBar';
import LoadingBox from '../LoadingBox';
import VideoEndpoint from '../../api/VideoEndpoint';
import StateContext from '../../utils/StateContext';
import MessageBox from '../MessageBox';
import VideoSearchResult from '../video/VideoSearchResult';
import PagedResultNavigator from '../PagedResultNavigator';
import Common from '../../utils/Common';
import { useTranslation } from 'react-i18next';

export default function SearchPage() {
    const [search, setSearch] = useState('');

    function onSearch(value) {
        if (value?.toLowerCase() !== search?.toLocaleLowerCase()) {
            setSearch(value ?? '');
        }
    }

    return (
        <Container sx={{ maxWidth: '100% !important' }}>
            <Container sx={{ maxWidth: '1152px !important' }}>
                <SearchBar sx={{ marginTop: 1 }} onSearch={onSearch} />
            </Container>
            <VideoSearchResult query={search} useRandomOnEmpryQuery usePager />
        </Container>
    );
}