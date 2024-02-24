import { useState, useEffect } from 'react';
import { InputBase, Paper, IconButton } from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';
import { useSearchParams } from 'react-router-dom';

export default function SearchBar({ sx, onSearch }) {
    const [value, setValue] = useState('');
    const [search, setSearch] = useSearchParams();

    function updateSearch() {
        if (value?.length > 0) {
            setSearch({ q: value });
        }
        else {
            search.delete('q');
            setSearch(search);
        }
    }

    function handleSearch(event) {
        event?.preventDefault && event.preventDefault();
        onSearch && onSearch(value);
        updateSearch();
    };

    function handleClear(event) {
        event?.preventDefault && event.preventDefault();
        setValue('');
        onSearch && onSearch('');
    }

    useEffect(() => {
        var q = search.get('q') ?? '';

        if (q !== value) {
            setValue(q);
            onSearch && onSearch(q);
        }
    })

    return (
        <Paper
            component="form"
            onSubmit={handleSearch}
            sx={{ ...sx, p: '2px 4px', display: 'flex', alignItems: 'center' }}
        >
            <InputBase
                sx={{ ml: 1, flex: 1 }}
                placeholder="Search..."
                inputProps={{ 'aria-label': 'search google maps' }}
                value={value}
                onChange={e => setValue(e.target.value)}
            />
            <IconButton type="submit" sx={{ p: '10px' }} aria-label="search"><SearchIcon /></IconButton>
            {value?.length > 0 && <IconButton type="button" sx={{ p: '10px' }} onClick={handleClear}><ClearIcon /></IconButton>}
        </Paper>
    );
};

