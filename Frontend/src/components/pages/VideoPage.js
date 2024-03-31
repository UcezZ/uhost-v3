import { Container } from '@mui/system';
import { useContext, useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import VideoEndpoint from '../../api/VideoEndpoint';
import Common from '../../utils/Common';
import StateContext from '../../utils/StateContext';
import LoadingBox from '../LoadingBox';
import VideoContainer from '../video/VideoContainer';
import NotFoundPage from './NotFoundPage';

export default function VideoPage() {
    const { token } = useParams();
    const { setError } = useContext(StateContext);
    const [video, setVideo] = useState();
    const [loading, setLoading] = useState(true);

    async function onLoad() {
        if (loading) {
            await VideoEndpoint.getByToken(token)
                .then(e => {
                    if (e?.data?.success && e?.data?.result) {
                        setVideo(e.data.result);
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
            <Container sx={{ maxWidth: '100% !important' }}>
                <LoadingBox fullscreen />
            </Container>
        );
    }

    if (video) {
        return (
            <Container sx={{ maxWidth: '100% !important' }}>
                <VideoContainer video={video} setVideo={setVideo} />
            </Container>
        );
    }

    return (
        <NotFoundPage />
    );
}