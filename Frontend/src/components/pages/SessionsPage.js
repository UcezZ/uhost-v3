import { Box, Button, Card, CardActions, CardContent, CircularProgress, Container, FormControl, Grid, InputLabel, MenuItem, Select, Typography } from '@mui/material';
import { useContext, useEffect, useState } from 'react';
import SessionEndpoint from '../../api/SessionEndpoint';
import Common from '../../utils/Common';
import StateContext from '../../utils/StateContext';
import PagedResultNavigator from '../common/PagedResultNavigator';
import SessionItem from '../sessions/SessionItem';
import LoadingBox from '../common/LoadingBox';
import { useTranslation } from 'react-i18next';
import NoDataBox from '../common/NoDataBox';
import SortDirSelect from '../common/SortDirSelect';
import PerPageSelect from '../common/PerPageSelect';

const SORT_BY_LIST = [
    'userid',
    'expiresin'
];

export default function SessionPage() {
    const [loading, setLoading] = useState(true);
    const [pager, setPager] = useState({ page: 1, perPage: 50 });
    const [items, setItems] = useState([]);
    const [sortDir, setSortDir] = useState(Common.getSortDirections()[1]);
    const [sortBy, setSortBy] = useState(SORT_BY_LIST[1]);
    const { setError } = useContext(StateContext);
    const { t } = useTranslation();

    async function onLoad() {
        if (!loading) {
            return;
        }

        await SessionEndpoint.get(pager?.page, pager?.perPage, sortBy, sortDir)
            .then(e => {
                if (e?.data?.success && e?.data?.result?.pager && e?.data?.result?.items) {
                    setPager(e.data.result.pager);
                    setItems(e.data.result.items);
                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));

        setLoading(false);
    }

    function onPageToggle(page) {
        setPager({ ...pager, page: page });
        setLoading(true);
    }

    useEffect(() => {
        onLoad();
    }, [loading]);

    return (
        <Container sx={{ maxWidth: '100% !important' }}>
            <Typography variant='h4' m={2}>{t('menu.sessions')}</Typography>
            <Box
                display='flex'
                justifyContent='space-around'
                width='100%'
                mb={1}>
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
                                {SORT_BY_LIST.map((e, i) => <MenuItem key={i} value={e}>{t(`filter.session.sortby.${e}`)}</MenuItem>)}
                            </Select>
                        </FormControl>

                        <SortDirSelect sortDir={sortDir} setSortDir={setSortDir} />

                        <PerPageSelect perPage={pager.perPage} setPerPage={e => setPager({ ...pager, perPage: e })} />
                    </CardContent>
                    <CardActions>

                        <Button
                            fullWidth
                            variant='contained'
                            disabled={loading}
                            sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                            onClick={e => onPageToggle(1)}
                        >
                            {
                                loading
                                    ? <CircularProgress size={20} />
                                    : t('common.apply')
                            }
                        </Button>
                    </CardActions>
                </Card>
            </Box>
            {
                loading
                    ? <LoadingBox />
                    : items.length > 0
                        ? <div>
                            <PagedResultNavigator sx={{ m: 1 }} pager={pager} onPageToggle={onPageToggle} />
                            <Grid container sx={{ justifyContent: 'space-around' }}>
                                {items.map((e, i) => <SessionItem key={i} item={e} onTerminated={e => setLoading(true)} />)}
                            </Grid>
                            <PagedResultNavigator sx={{ m: 1 }} pager={pager} onPageToggle={onPageToggle} />
                        </div>
                        : <NoDataBox />
            }
        </Container>
    );
}