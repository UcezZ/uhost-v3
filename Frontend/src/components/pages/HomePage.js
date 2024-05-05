import { Container } from '@mui/system';
import { useState } from 'react';
import SearchBar from '../common/SearchBar';
import VideoSearchResult from '../video/VideoSearchResult';
import { useTranslation } from 'react-i18next';
import { Typography } from '@mui/material';

export default function SearchPage() {
    const [search, setSearch] = useState('');
    const { t } = useTranslation();

    function onSearch(value) {
        if (value?.toLowerCase() !== search?.toLocaleLowerCase()) {
            setSearch(value ?? '');
        }
    }

    return (
        <Container sx={{ maxWidth: '100% !important' }}>
            <SearchBar sx={{ marginTop: 1 }} onSearch={onSearch} />
            {
                search?.length > 0
                    ? <div>
                        <Typography variant='h4' m={2}>{t('home.searchresult')}</Typography>
                        <VideoSearchResult query={search} usePager sortBy='CreatedAt' sortDir='Desc' />
                    </div>
                    : <div>
                        <Typography variant='h4' m={2}>{t('home.new')}</Typography>
                        <VideoSearchResult sortBy='CreatedAt' sortDir='Desc' perPage={10} />
                        <Typography variant='h4' m={2}>{t('home.random')}</Typography>
                        <VideoSearchResult useRandomOnEmpryQuery perPage={15} />
                    </div>
            }
        </Container>
    );
}