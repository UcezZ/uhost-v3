import { Box, Card, CardActions, CardContent, CircularProgress, Container, Button, FormControl, InputLabel, Select, MenuItem, Typography } from '@mui/material';
import { useContext, useEffect, useState } from 'react';
import RoleEndpoint from '../../api/RoleEndpoint';
import PagedResultNavigator from '../common/PagedResultNavigator';
import Common from '../../utils/Common';
import { useTranslation } from 'react-i18next';
import LoadingBox from '../common/LoadingBox';
import StateContext from '../../utils/StateContext';
import RoleItem from '../role/RoleItem';
import SortDirSelect from '../common/SortDirSelect';
import PerPageSelect from '../common/PerPageSelect';
import Rights from '../../utils/Rights';
import AddRoleDialogButton from '../role/AddRoleDialogButton';

const SORT_BY_LIST = [
    'id',
    'name',
    'createdat'
];

export default function RolePage({ events }) {
    const [loading, setLoading] = useState(true);
    const [pager, setPager] = useState({ page: 1, perPage: 25 });
    const [items, setItems] = useState([]);
    const [sortDir, setSortDir] = useState(Common.getSortDirections()[0]);
    const [sortBy, setSortBy] = useState(SORT_BY_LIST[0]);
    const { t } = useTranslation();
    const { setError, user } = useContext(StateContext);
    events ??= [];

    if (!events.length) {
        events.push('undefined');
    }

    async function onLoad() {
        if (!loading) {
            return;
        }

        await RoleEndpoint.getAll(pager?.page, pager?.perPage, sortBy, sortDir)
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
        <Container sx={{ maxWidth: '1280px !important' }}>
            <Typography variant='h4' m={2}>{t('menu.roles')}</Typography>
            <Box
                display='flex'
                justifyContent='space-around'
                width='100%'
                mb={2}>
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
                                {SORT_BY_LIST.map((e, i) => <MenuItem key={i} value={e}>{t(`filter.role.sortby.${e}`)}</MenuItem>)}
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
            {Rights.checkAnyRight(user, Rights.RoleCreateUpdate) && <AddRoleDialogButton next={e => onPageToggle(pager?.page ?? 1)} />}
            {
                loading
                    ? <LoadingBox />
                    : <div style={{
                        display: 'flex',
                        justifyContent: 'space-around',
                        flexWrap: 'wrap',
                        gap: '1em'
                    }}>
                        <PagedResultNavigator sx={{ m: 1 }} pager={pager} onPageToggle={onPageToggle} />
                        {items.map((e, i) => <RoleItem key={i} item={e} onUpdate={e => onPageToggle(pager?.page ?? 1)} />)}
                        <PagedResultNavigator sx={{ m: 1 }} pager={pager} onPageToggle={onPageToggle} />
                    </div>
            }

        </Container>
    );
}