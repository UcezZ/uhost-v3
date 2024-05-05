import { useState, useEffect } from 'react';
import { InputBase, Paper, IconButton, Container } from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';
import { useSearchParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

export default function SearchBar({ sx, onSearch }) {
    const { t } = useTranslation();
    const [value, setValue] = useState('');
    const [search, setSearch] = useSearchParams();

    function handleSearch(event) {
        event?.preventDefault && event.preventDefault();
        onSearch && onSearch(value);

        if (value?.length > 0) {
            setSearch({ q: value });
        }
        else {
            search.delete('q');
            setSearch(search);
        }
    };

    function handleClear(event) {
        event?.preventDefault && event.preventDefault();
        setValue('');
        onSearch && onSearch('');

        search.delete('q');
        setSearch(search);
    }

    useEffect(() => {
        var q = search.get('q') ?? '';

        if (q !== value) {
            setValue(q);
            onSearch && onSearch(q);
        }
    }, [search]);

    return (
        <Container sx={{ maxWidth: '1152px !important' }}>
            <Paper
                component='form'
                onSubmit={handleSearch}
                sx={{ ...sx, p: '2px 4px', display: 'flex', alignItems: 'center' }}
            >
                <InputBase
                    sx={{ ml: 1, flex: 1 }}
                    placeholder={t('common.search')}
                    value={value}
                    onChange={e => setValue(e.target.value)}
                />
                <IconButton type='submit' sx={{ p: '10px' }} aria-label='search'><SearchIcon /></IconButton>
                {value?.length > 0 && <IconButton type='button' sx={{ p: '10px' }} onClick={handleClear}><ClearIcon /></IconButton>}
            </Paper>
        </Container>
    );
};

