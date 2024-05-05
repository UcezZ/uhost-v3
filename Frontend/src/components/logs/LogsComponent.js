import { Box, Card, CardActions, CardContent, Checkbox, ListItemText, CircularProgress, Container, Button, FormControl, InputLabel, Select, MenuItem, Typography } from '@mui/material';
import { useContext, useEffect, useState } from 'react';
import LogsEndpoint from '../../api/LogsEndpoint';
import PagedResultNavigator from '../common/PagedResultNavigator';
import Common from '../../utils/Common';
import { useTranslation } from 'react-i18next';
import LoadingBox from '../common/LoadingBox';
import StateContext from '../../utils/StateContext';
import LogItem from '../logs/LogItem';
import NoDataBox from '../common/NoDataBox';
import SortDirSelect from '../common/SortDirSelect';
import PerPageSelect from '../common/PerPageSelect';

const SORT_BY_LIST = [
    'id',
    'eventid',
    'createdat',
    'ipaddress'
];

export default function LogsComponent({ events }) {
    const [loading, setLoading] = useState(true);
    const [pager, setPager] = useState({ page: 1, perPage: 100 });
    const [items, setItems] = useState([]);
    const [sortDir, setSortDir] = useState(Common.getSortDirections()[1]);
    const [sortBy, setSortBy] = useState(SORT_BY_LIST[2]);
    const [selectedEvents, setSelectedEvents] = useState([]);
    const { t } = useTranslation();
    const { setError } = useContext(StateContext);
    events ??= [];

    if (!events.length) {
        events.push('undefined');
    }

    async function onLoad() {
        if (!loading) {
            return;
        }

        await LogsEndpoint.get(pager?.page, pager?.perPage, sortBy, sortDir, selectedEvents)
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

    const eventLabelValue = selectedEvents.length > 0
        ? `${t('logs.events.caption')} (${selectedEvents.length})`
        : t('logs.events.caption')

    return (
        <Container sx={{ maxWidth: '1280px !important' }}>
            <Typography variant='h4' m={2}>{t('menu.logs')}</Typography>
            <Box
                display='flex'
                justifyContent='space-around'
                width='100%'
                mb={1}>
                <Card sx={{ flex: 1 }}>
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
                        {/* events */}
                        <FormControl sx={{ width: '100%' }}>
                            <InputLabel id='select-events'>{eventLabelValue}</InputLabel>
                            <Select
                                labelId='select-events'
                                multiple
                                value={selectedEvents}
                                onChange={e => setSelectedEvents(e.target.value.filter(v => events.includes(v)))}
                                label={eventLabelValue}
                                renderValue={e => e.map(k => t(`logs.events.${k.toString().toKebabCase()}`)).join('; ')}
                            // MenuProps={MenuProps}
                            >
                                {events.map((e, i) => (
                                    <MenuItem key={i} value={e}>
                                        <Checkbox checked={selectedEvents.includes(e)} />
                                        <ListItemText primary={t(`logs.events.${e.toString().toKebabCase()}`)} />
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>

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
                                {SORT_BY_LIST.map((e, i) => <MenuItem key={i} value={e}>{t(`filter.log.sortby.${e}`)}</MenuItem>)}
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
                            {items.map((e, i) => <LogItem key={i} item={e} />)}
                            <PagedResultNavigator sx={{ m: 1 }} pager={pager} onPageToggle={onPageToggle} />
                        </div>
                        : <NoDataBox />
            }

        </Container>
    );
}