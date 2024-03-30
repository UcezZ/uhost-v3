import { useState, useEffect, useContext } from 'react';
import { useParams } from 'react-router-dom';
import LoadingBox from '../LoadingBox';
import VideoEndpoint from '../../api/VideoEndpoint';
import StateContext from '../../utils/StateContext';
import NotFoundPage from './NotFoundPage';
import VideoProcessingItem from '../video-processing/VideoProcessingItem';
import { Container } from '@mui/material';

export default function VideoProcessingPage() {
    const { user } = useContext(StateContext);
    const { token } = useParams();
    const [loading, setLoading] = useState(true);
    const [processings, setProcessings] = useState([]);
    const [pager, setPager] = useState({});
    const [page, setPage] = useState(1);
    const [perPage, setPerPage] = useState(25);

    if (!user?.id) {
        return (
            <NotFoundPage />
        );
    }

    async function onLoad() {
        if (loading) {
            await VideoEndpoint.getAllProgresses(null, user.id, page, perPage, 'CreatedAt', 'Desc')
                .then(e => {
                    if (e?.data?.success && e?.data?.result?.pager && e?.data?.result?.items) {
                        setPager(e.data.result.pager);
                        setProcessings(e.data.result.items);
                    } else {
                        setError(Common.transformErrorData(e));
                    }
                }).catch(e => {
                    setError(Common.transformErrorData(e));
                });
            setLoading(false);
        }
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
        <Container sx={{ maxWidth: '1152px !important' }}>
            {processings.map((e, i) => <VideoProcessingItem key={i} video={e} />)}
        </Container>
    );
}