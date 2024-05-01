import { Card, CardActions, CardContent, Container, Button, Typography } from '@mui/material';
import { useContext, useEffect, useState } from 'react';
import LogsEndpoint from '../../api/LogsEndpoint';
import PagedResultNavigator from '../PagedResultNavigator';
import Common from '../../utils/Common';
import { useTranslation } from 'react-i18next';
import FilterAltIcon from '@mui/icons-material/FilterAlt';
import FilterAltOffIcon from '@mui/icons-material/FilterAltOff';
import LoadingBox from '../LoadingBox';
import StateContext from '../../utils/StateContext';
import LogItem from './LogItem';

export default function LogsComponent({ events }) {
    const [loading, setLoading] = useState(true);
    const [pager, setPager] = useState({});
    const [items, setItems] = useState([]);
    const [hasFilter, setHasFilter] = useState(false);
    const { t } = useTranslation();
    const { setError } = useContext(StateContext);

    async function onLoad() {
        if (!loading) {
            return;
        }
        console.log(pager);
        await LogsEndpoint.get(pager?.page, pager?.perPage)
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

    function onClearFilter() {
        setHasFilter(false);
    }

    function onApplyFilter() {
        setHasFilter(true);
        setPager({ ...pager, page: 1 });
    }

    function onPageToggle(page) {
        console.log(page)
        setPager({ ...pager, page: page });
        setLoading(true);
    }

    useEffect(() => {
        onLoad();
    }, [loading]);

    if (loading) {
        return (
            <LoadingBox />
        );
    }

    return (
        <Container sx={{ maxWidth: '1280px !important' }}>
            {/* <Card>
                <CardContent>

                </CardContent>
                <CardActions >
                    {
                        hasFilter && <Button
                            size='large'
                            color='inherit'
                            variant='contained'
                            fullWidth
                            onClick={onClearFilter}>
                            <FilterAltOffIcon />
                            {t('common.filter.clear')}
                        </Button>
                    }
                    <Button
                        size='large'
                        color='primary'
                        variant='contained'
                        fullWidth
                        onClick={onApplyFilter}>
                        <FilterAltIcon />
                        {t('common.filter.apply')}
                    </Button>
                </CardActions>
            </Card> */}
            <PagedResultNavigator sx={{ m: 1 }} pager={pager} onPageToggle={onPageToggle} />
            {/* <CodeBlock data={events} json /> */}
            {items.map((e, i) => <LogItem key={i} item={e} />)}
            <PagedResultNavigator sx={{ m: 1 }} pager={pager} onPageToggle={onPageToggle} />
        </Container>
    );
}