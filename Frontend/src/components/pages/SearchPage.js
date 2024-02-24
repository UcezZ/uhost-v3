import { CssBaseline } from "@mui/material";
import { Container } from "@mui/system";
import { useContext, useEffect, useState } from "react";
import SearchBar from "../items/SearchBar";
import LoadingBox from "../LoadingBox";
import VideoEndpoint from "../../api/VideoEndpoint";
import StateContext from "../../utils/StateContext";
import MessageBox from "../MessageBox";
import VideoContainer from "../VideoContainer";
import PagedResultNavigator from "../PagedResultNavigator";
import Common from "../../utils/Common";

export default function SearchPage() {
    const [loading, setLoading] = useState(false);
    const [videos, setVideos] = useState([]);
    const [pager, setPager] = useState();
    const [search, setSearch] = useState('');
    const { user, setError } = useContext(StateContext);

    function onSearch(value) {
        if (value?.toLowerCase() !== search?.toLocaleLowerCase()) {
            setSearch(value ?? '');
            setLoading(true);
        }
    }

    useEffect(() => {
        console.log(search);
        if (loading) {
            var useSearch = search?.length > 0;
            (useSearch ? VideoEndpoint.search(search, pager?.currentPage ?? 1, 3) : VideoEndpoint.random())
                .then(e => {
                    if (e?.data?.success && e?.data?.result) {
                        if (useSearch) {
                            setVideos(e.data.result.items);
                            setPager(e.data.result.pager);
                        } else {
                            setVideos(e.data.result);
                            setPager();
                        }
                    } else {
                        setError(Common.transformErrorData(e));
                    }
                    setLoading(false);
                })
                .catch(e => {
                    setLoading(false);
                    setVideos([]);
                    setPager();
                    setError(Common.transformErrorData(e));
                });
        }
    }, [loading]);

    useEffect(() => setLoading(true), [user]);

    function onPageToggle(page) {
        setPager({ ...pager, currentPage: page });
        setLoading(true);
    }

    return (
        <Container sx={{ maxWidth: '100% !important' }}>
            <CssBaseline />
            <Container sx={{ maxWidth: '1152px !important' }}>
                <SearchBar sx={{ marginTop: 1 }} onSearch={onSearch} />
            </Container>
            <Container sx={{ maxWidth: '100% !important' }}>
                {
                    loading
                        ? <LoadingBox />
                        : videos?.length > 0
                            ? <VideoContainer videos={videos} pager={pager} onPageToggle={onPageToggle} />
                            : <MessageBox text='Не найдено ни одного видео по запросу' />
                }
                {pager?.totalPages > 1 && <PagedResultNavigator pager={pager} onPageToggle={onPageToggle} sx={{ maxWidth: '1280px' }} />}
            </Container>
        </Container>
    );
}