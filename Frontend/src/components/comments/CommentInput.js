import { Card, Container, Avatar, InputBase, Typography, IconButton, Paper, Divider, Grid, CardContent, Button, CircularProgress, useMediaQuery } from '@mui/material';
import { useContext, useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import CommentEndpoint from '../../api/CommentEndpoint';
import StateContext from '../../utils/StateContext';
import CommentItem from './CommentItem';
import AddCommentIcon from '@mui/icons-material/AddComment';
import ClearIcon from '@mui/icons-material/Clear';
import Common from '../../utils/Common';

export default function CommentInput({ videoToken, onCommentPosted }) {
    const { t } = useTranslation();
    const { user, setError } = useContext(StateContext);
    const [loading, setLoading] = useState(false);
    const [text, setText] = useState('');
    const isNarrowScreen = useMediaQuery('(max-width:600px)');

    async function onPost() {
        if (loading) {
            return;
        }
        await CommentEndpoint.post(videoToken, text)
            .then(e => {
                if (e?.data?.success && e?.data?.result) {
                    onCommentPosted && onCommentPosted(e.data.result);
                    setText('');
                } else {
                    setError(Common.transformErrorData(e));
                }
            }).catch(e => {
                setError(Common.transformErrorData(e));
            });
        setLoading(false);
    }

    function postComment(event) {
        event?.preventDefault && event.preventDefault();
        setLoading(true);
        onPost();
    }

    const icon = loading ? <CircularProgress size={20} /> : <AddCommentIcon />

    return (
        <Paper
            component='form'
            onSubmit={postComment}
            sx={{
                mt: 2,
                p: '2px 4px',
                display: 'flex',
                alignItems: 'center',
                flex: 1
            }}
        >
            <InputBase
                sx={{ ml: 1, flex: 1 }}
                placeholder={t('comments.input')}
                value={text}
                onChange={e => setText(e.target.value)}
            />
            {text?.length > 0 && <IconButton type='button' sx={{ p: '10px' }} onClick={e => setText('')}><ClearIcon /></IconButton>}
            <Button
                size='large'
                variant='contained'
                color='primary'
                type='submit'
                disabled={loading || text?.length < 3}
                startIcon={!isNarrowScreen && icon}
                sx={{ p: '2 1' }} >
                {isNarrowScreen && icon}
                {!isNarrowScreen && t('comment.send')}
            </Button>
        </Paper>
    );
}