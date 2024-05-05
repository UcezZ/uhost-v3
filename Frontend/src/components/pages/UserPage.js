import { Box, Card, CardActions, CardContent, Checkbox, ListItemText, CircularProgress, Container, Button, FormControl, InputLabel, Select, MenuItem, Typography } from '@mui/material';
import { useContext, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import UserEndpoint from '../../api/UserEndpoint';
import StateContext from '../../utils/StateContext';
import LoadingBox from '../common/LoadingBox';
import Common from '../../utils/Common';
import UserItem from '../user/UserItem';
import NoDataBox from '../common/NoDataBox';
import SortDirSelect from '../common/SortDirSelect';
import PerPageSelect from '../common/PerPageSelect';
import PagedResultNavigator from '../common/PagedResultNavigator';

const SORT_BY_LIST = [
    'id',
    'createdat',
    'lastvisitat'
];

export default function UserPage() {
    const [loading, setLoading] = useState(true);
    const [items, setItems] = useState([]);
    const [pager, setPager] = useState({ page: 1, perPage: 25 });
    const [sortDir, setSortDir] = useState(Common.getSortDirections()[0]);
    const [sortBy, setSortBy] = useState(SORT_BY_LIST[0]);
    const { t } = useTranslation();
    const { setError } = useContext(StateContext);

    async function onLoad() {
        if (!loading) {
            return;
        }

        await UserEndpoint.getAll(pager?.page, pager?.perPage, sortBy, sortDir)
            .then(e => {
                if (e?.data?.success && e?.data?.result?.items && e?.data?.result?.pager) {
                    setItems(e.data.result.items);
                    setPager(e.data.result.pager)
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
        <Container sx={{ maxWidth: '1280px !important' }}>
            <Typography variant='h4' m={2}>{t('menu.users')}</Typography>
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
                                {SORT_BY_LIST.map((e, i) => <MenuItem key={i} value={e}>{t(`filter.user.sortby.${e}`)}</MenuItem>)}
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
                        ? <div style={{
                            display: 'flex',
                            justifyContent: 'space-around',
                            flexWrap: 'wrap',
                            gap: '1em'
                        }}>
                            <PagedResultNavigator sx={{ m: 1 }} pager={pager} onPageToggle={onPageToggle} />
                            {items.map((e, i) => <UserItem key={i} item={e} onUpdate={u => onPageToggle(1)} />)}
                            <PagedResultNavigator sx={{ m: 1 }} pager={pager} onPageToggle={onPageToggle} />
                        </div>
                        : <NoDataBox />
            }

        </Container>
    );
}