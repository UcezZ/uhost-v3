import React, { useContext, useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import FormControlLabel from '@mui/material/FormControlLabel';
import Checkbox from '@mui/material/Checkbox';
import Link from '@mui/material/Link';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import AuthEndpoint from '../../api/AuthEndpoint';
import StateContext from '../../utils/StateContext';
import { CircularProgress } from '@mui/material';
import Common from '../../utils/Common';
import VideoEndpoint from '../../api/VideoEndpoint';

const noSelectSx = {
    userSelect: false,
    msUserSelect: false,
    MozUserSelect: false,
    msTouchSelect: false,
    WebkitUserSelect: false
};

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
                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));
        setLoading(false);

        next && next(event);
    }

    function isNameError() {
        return name?.length < 3 || name?.length > 64;
    }

    function isDescError() {
        return desc && desc.length > 512;
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
                    error={isNameError()}
                    disabled={loading}
                    value={name}
                    onChange={e => setName(e.target.value)}
                    autoFocus
                />
                <TextField
                    margin='normal'
                    fullWidth
                    label='Описание'
                    error={isDescError()}
                    disabled={loading}
                    value={desc}
                    onChange={e => setDesc(e.target.value)}
                    minRows={3}
                    maxRows={10}
                    multiline
                />
                <FormControlLabel
                    control={<Checkbox color='primary' checked={isPrivate} onClick={e => setIsPrivate(!isPrivate)} />}
                    label='Скрыть из общего доступа'
                    sx={noSelectSx}
                />
                <FormControlLabel
                    control={<Checkbox color='primary' checked={isHidden} onClick={e => setIsHidden(!isHidden)} />}
                    label='Скрыть из результатов поиска'
                    sx={noSelectSx}
                />
                <FormControlLabel
                    control={<Checkbox color='primary' checked={allowComments} onClick={e => setAllowComments(!allowComments)} />}
                    label='Разрешить комментарии'
                    sx={noSelectSx}
                />
                <FormControlLabel
                    control={<Checkbox color='primary' checked={allowReactions} onClick={e => setAllowReactions(!allowReactions)} />}
                    label='Разрешить реакции'
                    sx={noSelectSx}
                />
                <Box sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    alignItems: 'center',
                    gap: 2
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
                        disabled={loading}
                        sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                    >
                        {loading ? <CircularProgress size={20} /> : 'Применить'}
                    </Button>
                </Box>
            </Box>
        </Box>
    );
}