import { Box, Card, CardContent, CardActions, Container, FormControl, InputLabel, Select, MenuItem, Typography, Button } from '@mui/material';
import { useContext, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useParams } from 'react-router-dom';
import Common from '../../utils/Common';
import StateContext from '../../utils/StateContext';
import SearchBar from '../common/SearchBar';
import AddVideoDialogButton from '../video/AddVideoDialogButton';
import VideoSearchResult from '../video/VideoSearchResult';
import NotFoundPage from './NotFoundPage';
import SortDirSelect from '../common/SortDirSelect';
import PerPageSelect from '../common/PerPageSelect';

const SORT_BY_LIST = [
    'name',
    'duration',
    'createdat'
];

export default function Videos() {
    const { t } = useTranslation();
    const { user } = useContext(StateContext);
    const [search, setSearch] = useState('');
    const [sortBy, setSortBy] = useState(SORT_BY_LIST[1]);
    const [sortDir, setSortDir] = useState(Common.getSortDirections()[1]);
    const [perPage, setPerPage] = useState(25);
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
            <Typography variant='h4' m={2}>{isSameUser ? t('video.my') : t('video.ofuser', { user: targetLogin })}</Typography>
            {
                user?.id > 0 && <Box
                    display='flex'
                    justifyContent='space-around'
                    width='100%'
                    mb={1}
                    mt={2}>
                    <Card>
                        <CardContent>
                            <Typography variant='h6'>{t('filter.caption')}</Typography>
                        </CardContent>
                        <CardContent sx={{
                            display: 'flex',
                            flexDirection: 'row',
                            flexWrap: 'wrap',
                            justifyContent: 'space-around',
                            gap: '1em'
                        }}>

                            {/* sort by */}
                            <FormControl>
                                <InputLabel id='select-sort-by'>{t('filter.common.sortby.caption')}</InputLabel>
                                <Select
                                    labelId='select-sort-by'
                                    value={sortBy}
                                    label={t('filter.common.sortby.caption')}
                                    sx={{ minWidth: '160px' }}
                                    onChange={e => setSortBy(Common.filterStringArrayValue(SORT_BY_LIST, e.target.value, 1))}
                                >
                                    {SORT_BY_LIST.map((e, i) => <MenuItem key={i} value={e}>{t(`filter.video.sortby.${e}`)}</MenuItem>)}
                                </Select>
                            </FormControl>

                            <SortDirSelect sortDir={sortDir} setSortDir={setSortDir} />

                            <PerPageSelect perPage={perPage} setPerPage={setPerPage} />
                        </CardContent>
                    </Card>
                </Box>
            }
            {
                user?.id > 0 && <AddVideoDialogButton />
            }
            <VideoSearchResult
                userLogin={targetLogin}
                query={search}
                sortBy={sortBy}
                sortDir={sortDir}
                perPage={perPage}
                showHidden
                showPrivate
                usePager />
        </Container>
    );
}