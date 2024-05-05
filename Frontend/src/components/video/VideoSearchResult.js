import { Container, Grid } from '@mui/material';
import VideoPreview from './VideoPreview';
import { useContext, useEffect, useState } from 'react';
import LoadingBox from '../common/LoadingBox';
import VideoEndpoint from '../../api/VideoEndpoint';
import StateContext from '../../utils/StateContext';
import MessageBox from '../common/MessageBox';
import PagedResultNavigator from '../common/PagedResultNavigator';
import Common from '../../utils/Common';
import { useTranslation } from 'react-i18next';

export default function VideoSearchResult({ query, userLogin, usePager, useRandomOnEmpryQuery, perPage, sortBy, sortDir, showHidden, showPrivate }) {
    const { t } = useTranslation();
    const [loading, setLoading] = useState(false);
    const [videos, setVideos] = useState([]);
    const [pager, setPager] = useState({ page: 1, perPage: perPage ?? 25 });
    const { user, setError } = useContext(StateContext);

    function onPageToggle(page) {
        setPager({ ...pager, currentPage: page });
        setLoading(true);
    }

    useEffect(() => setLoading(true), [user, query, userLogin, usePager, useRandomOnEmpryQuery, perPage, sortBy, sortDir, showHidden, showPrivate]);

    async function onLoad() {
        if (loading) {
            var q = query?.trim && query?.trim();

            var useRandom = !q?.length && useRandomOnEmpryQuery;
            var promise = useRandom
                ? VideoEndpoint.random(perPage)
                : VideoEndpoint.search(
                    q,
                    usePager && pager?.currentPage > 0 ? pager.currentPage : 1,
                    perPage,
                    sortBy,
                    sortDir,
                    showHidden,
                    showPrivate,
                    userLogin);

            await promise
                .then(e => {
                    if (e?.data?.success && e?.data?.result) {
                        if (useRandom) {
                            setVideos(e.data.result);
                            setPager();
                        } else {
                            setVideos(e.data.result.items);
                            setPager(e.data.result.pager);
                        }
                    } else {
                        setError(Common.transformErrorData(e));
                    }
                })
                .catch(e => {
                    setVideos([]);
                    setPager();
                    setError(Common.transformErrorData(e));
                });

            setLoading(false);
        }
    }

    useEffect(() => {
        onLoad();
    }, [loading]);

    return (
        <Container sx={{ maxWidth: '100% !important' }}>
            {usePager && pager?.totalPages > 1 && <PagedResultNavigator pager={pager} onPageToggle={onPageToggle} sx={{ maxWidth: '1280px' }} />}
            {
                loading
                    ? <LoadingBox />
                    : videos?.length > 0
                        ? <Grid container sx={{ justifyContent: 'space-around' }}>
                            {videos?.map && videos.map((e, i) => <VideoPreview entity={e} key={i} />)}
                        </Grid>
                        : <MessageBox text={t('video.search.notfound')} />
            }
            {usePager && pager?.totalPages > 1 && <PagedResultNavigator pager={pager} onPageToggle={onPageToggle} sx={{ maxWidth: '1280px' }} />}
        </Container>
    );
}