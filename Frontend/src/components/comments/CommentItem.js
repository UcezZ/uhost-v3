import { Button, Avatar, Typography, CircularProgress, Grid, CardContent, useMediaQuery, Divider } from '@mui/material';
import { useContext, useState } from 'react';
import { useTranslation } from 'react-i18next';
import CommentEndpoint from '../../api/CommentEndpoint';
import StateContext from '../../utils/StateContext';
import SafeImage from '../common/SafeImage';
import DeleteIcon from '@mui/icons-material/Delete';
import YesNoDialog from '../common/YesNoDialog';
import Common from '../../utils/Common';
import { red } from '@mui/material/colors';

export default function CommentItem({ videoToken, comment, onCommentDeleted }) {
    const { t } = useTranslation();
    const { user, setError } = useContext(StateContext);
    const [loading, setLoading] = useState(false);
    const [visible, setVisible] = useState(false);
    var login = comment?.user?.login?.length ? comment.user.login : 'N/A' ?? 'N/A';
    var avaText = login.at(0).toString().toUpperCase();
    var createdAt = comment?.createdAt ?? 'N/A';
    const isNarrowScreen = useMediaQuery('(max-width:600px)');

    const canDeleteComment = user?.id && user.id === comment?.user?.id;

    async function onDelete() {
        if (loading) {
            return;
        }
        await CommentEndpoint.delete(videoToken, comment.id)
            .then(e => {
                if (e?.data?.success && e?.data?.result) {
                    onCommentDeleted && onCommentDeleted(comment.id);
                } else {
                    setError(Common.transformErrorData(e));
                }
            }).catch(e => {
                setError(Common.transformErrorData(e));
            });
        setLoading(false);
    }

    function onDeleteConfirm() {
        setLoading(true);
        onDelete();
    }

    const icon = loading ? <CircularProgress size={20} /> : <DeleteIcon />
    const deleteText = comment?.text?.length > 32 ? `${comment.text.substring(0, 32)}...` : comment.text;

    return (
        <Grid
            container
            wrap='nowrap'
            columnSpacing={2}
            rowSpacing={0}
            p={1}
            id={`comment-${comment.id}`} >
            <Grid item>
                <Avatar sx={{ bgcolor: comment?.user?.avatarUrl?.length > 0 ? '#0000' : red[500] }} aria-label='recipe'>
                    {
                        comment?.user?.avatarUrl?.length > 0
                            ? <SafeImage src={comment.user.avatarUrl} />
                            : avaText
                    }
                </Avatar>
            </Grid>
            <Grid item xs flex={1}>
                <Typography variant='subtitle1' component='div'>
                    {login}, {createdAt}
                </Typography>
                <Typography variant='body2' component='div'>
                    {comment.text}
                </Typography>
            </Grid>
            {
                canDeleteComment && <Grid item>
                    <Button
                        size='large'
                        variant='contained'
                        color='primary'
                        onClick={e => setVisible(true)}
                        disabled={loading}
                        startIcon={!isNarrowScreen && icon}
                        sx={{ p: '2 1' }} >
                        {isNarrowScreen && icon}
                        {!isNarrowScreen && t('comments.delete')}
                    </Button>
                </Grid>
            }
            <YesNoDialog onYes={onDeleteConfirm} message={t('comments.delete.confirm', { text: deleteText })} visible={visible} setVisible={setVisible} />
        </Grid>
    )
}