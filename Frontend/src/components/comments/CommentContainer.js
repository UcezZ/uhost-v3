import { Card, Avatar, Typography, Paper, Divider, Grid, CardContent, CardActions } from '@mui/material';
import { useContext, useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import CommentEndpoint from '../../api/CommentEndpoint';
import Common from '../../utils/Common';
import StateContext from '../../utils/StateContext';
import LoadingBox from '../common/LoadingBox';
import PagedResultNavigator from '../common/PagedResultNavigator';
import CommentInput from './CommentInput';
import CommentItem from './CommentItem';

export default function CommentContainer({ video }) {
    const { t } = useTranslation();
    const { user, setError } = useContext(StateContext);
    const [loading, setLoading] = useState(true);
    const [comments, setComments] = useState([]);
    const [pager, setPager] = useState({ page: 1, perPage: 15 });

    async function onLoad() {
        if (loading) {
            await CommentEndpoint.get(video?.token, pager?.page ?? 1, pager?.perPage ?? 15)
                .then(e => {
                    if (e?.data?.success && e?.data?.result?.pager && e?.data?.result?.items) {
                        setComments(e.data.result.items);
                        setPager(e.data.result.pager);
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

    async function onCommentPosted(comment) {
        setComments([comment, ...comments]);
        await Common.sleep(50);
        Common.scrollToElementById(`comment-${comment.id}`);
    }

    function onCommentDeleted(id) {
        setComments(comments.filter(e => e?.id !== id));

        if (comments?.length <= 1) {
            if (pager?.page === pager?.totalPages) {
                setPager({ ...pager, page: pager?.page - 1 });
            } else {
                setLoading(true);
            }
        }
    }

    function onPageToggle(page) {
        setPager({ ...pager, page: page });
        setLoading(true);
    }

    const fuckMirea = pager?.totalPages > 1 && <PagedResultNavigator pager={pager} onPageToggle={onPageToggle} sx={{ maxWidth: 600 }} />

    return (
        <Card sx={{ mt: 3, mb: 3 }} >
            <CardContent>
                <Typography variant="h6" gutterBottom>
                    {t('comments.caption')}
                </Typography>
            </CardContent>
            <Divider />
            <CardContent>
                {fuckMirea}
                {
                    loading
                        ? <LoadingBox />
                        : comments?.length > 0
                            ? comments.map((e, i) => <CommentItem key={i} comment={e} videoToken={video.token} onCommentDeleted={onCommentDeleted} />)
                            : <Typography flex={1}>{t('comments.empty')}</Typography>
                }
                {fuckMirea}
            </CardContent>
            {
                user?.id > 0 && <div>
                    <Divider />
                    <CardActions>
                        <CommentInput videoToken={video?.token} onCommentPosted={onCommentPosted} />
                    </CardActions>
                </div>
            }
        </Card >
    );
}