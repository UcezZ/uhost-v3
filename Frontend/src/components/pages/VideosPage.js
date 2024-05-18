import { Box, Card, CardContent, FormControlLabel, Checkbox, Container, FormControl, InputLabel, Select, MenuItem, Typography, Button } from '@mui/material';
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
import Rights from '../../utils/Rights';
import Styles from '../../ui/Styles';

const SORT_BY_LIST = [
    'name',
    'createdat',
    'duration'
];

export default function VideosPage() {
    const { t } = useTranslation();
    const { user } = useContext(StateContext);
    const [search, setSearch] = useState('');
    const [sortBy, setSortBy] = useState(SORT_BY_LIST[1]);
    const [sortDir, setSortDir] = useState(Common.getSortDirections()[1]);
    const [perPage, setPerPage] = useState(25);
    const [showHidden, setShowHidden] = useState(false);
    const [showPrivate, setShowPrivate] = useState(false);
    const [pager, setPager] = useState();
    const { login } = useParams();

    const targetLogin = login ?? user?.login;
    const isSameUser = targetLogin === user?.login;
    const canFilterHidden = Rights.checkAnyRight(user, Rights.VideoGetAll) || isSameUser;

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

    var label = isSameUser ? t('video.my') : t('video.ofuser', { user: targetLogin });

    if (pager?.total > 0) {
        label += ` (${pager.total})`;
    }

    return (
        <Container sx={{ maxWidth: '100% !important' }}>
            <SearchBar sx={{ marginTop: 1 }} onSearch={onSearch} />
            <Typography variant='h4' m={2}>{label}</Typography>
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
                            gap: '1em',
                            maxWidth: '48em'
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

                            {
                                canFilterHidden && <div style={{ textAlign: 'center' }}>
                                    <FormControlLabel
                                        control={<Checkbox color='primary' checked={showHidden || showPrivate} onClick={e => setShowHidden(!showHidden)} />}
                                        label={t('video.showhidden')}
                                        sx={Styles.noSelectSx}
                                        disabled={showPrivate}
                                    />
                                    <FormControlLabel
                                        control={<Checkbox color='primary' checked={showPrivate} onClick={e => setShowPrivate(!showPrivate)} />}
                                        label={t('video.showprivate')}
                                        sx={Styles.noSelectSx}
                                    />
                                </div>
                            }
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
                showHidden={showHidden || showPrivate}
                showPrivate={showPrivate}
                usePager
                onPagerFetched={e => setPager(e)} />
        </Container>
    );
}