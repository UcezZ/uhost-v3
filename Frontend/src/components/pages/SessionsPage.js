import { Container, Grid } from '@mui/material';
import { useContext, useEffect, useState } from 'react';
import SessionEndpoint from '../../api/SessionEndpoint';
import Common from '../../utils/Common';
import StateContext from '../../utils/StateContext';
import PagedResultNavigator from '../PagedResultNavigator';
import SessionItem from '../sessions/SessionItem';
import LoadingBox from '../LoadingBox';

export default function SessionPage() {
    const [loading, setLoading] = useState(true);
    const [pager, setPager] = useState({});
    const [items, setItems] = useState([]);
    const { setError } = useContext(StateContext);

    async function onLoad() {
        if (!loading) {
            return;
        }
        console.log(pager);
        await SessionEndpoint.get(pager?.page, pager?.perPage)
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
        <Container sx={{ maxWidth: '100% !important' }}>
            <PagedResultNavigator sx={{ m: 1 }} pager={pager} onPageToggle={onPageToggle} />
            <Grid container sx={{ justifyContent: 'space-around' }}>
                {items.map((e, i) => <SessionItem key={i} item={e} onTerminated={e => setLoading(true)} />)}
            </Grid>
            <PagedResultNavigator sx={{ m: 1 }} pager={pager} onPageToggle={onPageToggle} />
        </Container>
    );
}