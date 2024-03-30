import React, { useContext, useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import FormControlLabel from '@mui/material/FormControlLabel';
import Checkbox from '@mui/material/Checkbox';
import Box from '@mui/material/Box';
import StateContext from '../../utils/StateContext';
import { CircularProgress } from '@mui/material';
import Common from '../../utils/Common';
import VideoEndpoint from '../../api/VideoEndpoint';
import Validation from '../../utils/Validation';
import Styles from '../../ui/Styles';

export default function EditVideoForm({ video, setVideo, next }) {
    const { setError } = useContext(StateContext);
    const [loading, setLoading] = useState(false);
    const [name, setName] = useState(video?.name ?? '');
    const [desc, setDesc] = useState(video?.description ?? '');
    const [isPrivate, setIsPrivate] = useState(video?.isPrivate ?? false);
    const [isHidden, setIsHidden] = useState(video?.isHidden ?? false);
    const [allowComments, setAllowComments] = useState(video?.allowComments ?? true);
    const [allowReactions, setAllowReactions] = useState(video?.allowReactions ?? true);

    async function onSubmit(event) {
        event?.preventDefault && event.preventDefault();

        setLoading(true);
        await VideoEndpoint.edit(video.token, name, desc, isPrivate, isHidden, allowComments, allowReactions)
            .then(e => {
                if (e?.data?.success && e?.data?.result) {
                    setVideo(e.data.result);
                    e.data.result?.name && setName(e.data.result.name);
                    e.data.result?.description && setDesc(e.data.result.description);
                    e.data.result?.isPrivate && setIsPrivate(e.data.result.isPrivate);
                    e.data.result?.isHidden && setIsHidden(e.data.result.isHidden);
                    e.data.result?.allowComments && setAllowComments(e.data.result.allowComments);
                    e.data.result?.allowReactions && setAllowReactions(e.data.result.allowReactions);
                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));
        setLoading(false);

        next && next(event);
    }

    function isValid() {
        return Validation.Video.name(name) && Validation.Video.desc(desc);
    }

    return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
            }}
        >
            <Box component='form' noValidate onSubmit={onSubmit} sx={{ mt: 1 }}>
                <TextField
                    margin='normal'
                    required
                    fullWidth
                    label='Наименование'
                    error={!Validation.Video.name(name)}
                    disabled={loading}
                    value={name}
                    onChange={e => setName(e.target.value)}
                    autoFocus
                />
                <TextField
                    margin='normal'
                    fullWidth
                    label='Описание'
                    error={!Validation.Video.desc(desc)}
                    disabled={loading}
                    value={desc}
                    onChange={e => setDesc(e.target.value)}
                    minRows={3}
                    maxRows={10}
                    multiline
                />
                <FormControlLabel
                    control={<Checkbox color='primary' checked={isPrivate} onClick={e => {
                        setIsPrivate(!isPrivate);
                        if (!isPrivate) {
                            setIsHidden(true);
                        }
                    }} />}
                    label='Скрыть из общего доступа'
                    style={Styles.noSelectSx}
                />
                <FormControlLabel
                    control={<Checkbox color='primary' disabled={isPrivate} checked={isHidden} onClick={e => setIsHidden(!isHidden)} />}
                    label='Скрыть из результатов поиска'
                    sx={Styles.noSelectSx}
                />
                <FormControlLabel
                    control={<Checkbox color='primary' checked={allowComments} onClick={e => setAllowComments(!allowComments)} />}
                    label='Разрешить комментарии'
                    sx={Styles.noSelectSx}
                />
                <FormControlLabel
                    control={<Checkbox color='primary' checked={allowReactions} onClick={e => setAllowReactions(!allowReactions)} />}
                    label='Разрешить реакции'
                    sx={Styles.noSelectSx}
                />
                <Box sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    alignItems: 'center',
                    gap: 2,
                    ...Styles.noSelectSx
                }}>
                    <Button
                        fullWidth
                        variant='outlined'
                        disabled={loading}
                        sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                        onClick={next}
                    >
                        Отмена
                    </Button>
                    <Button
                        type='submit'
                        fullWidth
                        variant='contained'
                        disabled={loading || !isValid()}
                        sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                    >
                        {loading ? <CircularProgress size={20} /> : 'Применить'}
                    </Button>
                </Box>
            </Box>
        </Box>
    );
}