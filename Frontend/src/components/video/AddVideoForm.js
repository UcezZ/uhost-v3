import React, { useContext, useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import FormControlLabel from '@mui/material/FormControlLabel';
import Checkbox from '@mui/material/Checkbox';
import Box from '@mui/material/Box';
import StateContext from '../../utils/StateContext';
import { CircularProgress, Divider, IconButton, LinearProgress, Tab, Tabs, Typography } from '@mui/material';
import Common from '../../utils/Common';
import VideoEndpoint from '../../api/VideoEndpoint';
import Valitation from '../../utils/Validation';
import Styles from '../../ui/Styles';
import TabPanel from '../TabPanel';
import VideoFileIcon from '@mui/icons-material/VideoFile';
import CloseIcon from '@mui/icons-material/Close';
import VideoPreview from './VideoPreview';

const MAX_FILE_SIZE = 8589934591;

export default function AddVideoForm({ next, setCanClose }) {
    const { setError } = useContext(StateContext);
    const [loading, setLoading] = useState(false);
    const [name, setName] = useState('');
    const [desc, setDesc] = useState('');
    const [isPrivate, setIsPrivate] = useState(false);
    const [isHidden, setIsHidden] = useState(false);
    const [allowComments, setAllowComments] = useState(true);
    const [allowReactions, setAllowReactions] = useState(true);
    const [source, setSource] = useState(0);
    const [uploadProgress, setUploadProgress] = useState();
    const [videoFile, setVideoFile] = useState(null);
    const [videoUrl, setVideoUrl] = useState('');
    const [video, setVideo] = useState();
    const [maxDuration, setMaxDuration] = useState('01:00:00');

    async function onSubmit(event) {
        event?.preventDefault && event.preventDefault();

        setLoading(true);

        var promise;

        switch (source) {
            case 0:
                promise = VideoEndpoint.addFile(videoFile, name, desc, isPrivate, isHidden, allowComments, allowReactions, setUploadProgress);
                break;
            case 1:
                promise = VideoEndpoint.addUrl(videoUrl, maxDuration, name, desc, isPrivate, isHidden, allowComments, allowReactions, setUploadProgress);
                break;
            default:
                promise = new Promise(() => { });
                break;
        }

        await promise
            .then(e => {
                if (e?.data?.success && e?.data?.result) {
                    setVideo(e.data.result);
                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));

        setLoading(false);
    }

    function onFileSelected(e) {
        var selectedFile = e?.target?.files?.length && e.target.files[0];

        if (selectedFile !== videoFile) {
            setVideoFile(selectedFile);

            if (!name?.trim()?.length && selectedFile?.name?.length) {
                var selectedName = selectedFile.name.lastIndexOf
                    ? selectedFile.name.substring(0, selectedFile.name.lastIndexOf('.'))
                    : selectedFile.name;
                selectedName = selectedName.trimAll();
                setName(selectedName);
            }
        }
    }

    function isValid() {
        return Valitation.Video.name(name) && Valitation.Video.desc(desc) &&
            (
                source === 0 && videoFile ||
                source === 1 && Valitation.Video.url(videoUrl) && Valitation.Video.maxDuration(maxDuration)
            );
    }

    function nextAndReset() {
        next && next();
        setLoading();
        setName('');
        setDesc('');
        setIsPrivate(false);
        setIsHidden(false);
        setAllowComments(true);
        setAllowReactions(true);
        setSource(0);
        setUploadProgress();
        setVideoFile();
        setVideoUrl('');
        setVideo();
        setMaxDuration('01:00:00');
    }

    // если видео загрузилось
    if (video) {
        return (
            <Box
                sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    minWidth: '32em'
                }}
            >
                <VideoPreview entity={video} />
                <Typography>Видео успешно загружено</Typography>
                <Typography variant='caption'>Дождитель окончания обработки видео</Typography>
                <Button
                    color='success'
                    variant='contained'
                    sx={{ m: 2 }}
                    onClick={nextAndReset}>
                    Закрыть
                </Button>
            </Box>
        );
    }

    // если загружается файл
    if (loading && source === 0) {
        return (
            <Box
                sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    minWidth: '32em'
                }}
            >
                <Typography>Идёт загрузка файла</Typography>
                <Box sx={{
                    display: 'flex',
                    alignItems: 'center',
                    gap: 1,
                    width: '100%'
                }}>
                    <LinearProgress
                        sx={{
                            flex: 1,
                            height: 16,
                            borderRadius: 16
                        }}
                        variant='determinate'
                        value={uploadProgress?.progress * 100 ?? 0} />
                    <Typography variant="body2" color="text.secondary">{`${Math.round(
                        uploadProgress?.progress * 100 ?? 0,
                    )}%`}</Typography>
                </Box>
                <Typography>{Common.sizeToHuman(uploadProgress?.rate ?? 0)}/s, {Common.sizeToHuman(uploadProgress?.loaded ?? 0)} / {Common.sizeToHuman(uploadProgress?.total ?? 0)}</Typography>
            </Box>
        );
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
                    error={!Valitation.Video.name(name)}
                    disabled={loading}
                    value={name}
                    onChange={e => setName(e.target.value)}
                    autoFocus
                />
                <TextField
                    margin='normal'
                    fullWidth
                    label='Описание'
                    error={!Valitation.Video.desc(desc)}
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
                    disabled={loading}
                />
                <FormControlLabel
                    control={<Checkbox color='primary' checked={isHidden} onClick={e => setIsHidden(!isHidden)} />}
                    label='Скрыть из результатов поиска'
                    sx={Styles.noSelectSx}
                    disabled={loading || isPrivate}
                />
                <FormControlLabel
                    control={<Checkbox color='primary' checked={allowComments} onClick={e => setAllowComments(!allowComments)} />}
                    label='Разрешить комментарии'
                    sx={Styles.noSelectSx}
                    disabled={loading}
                />
                <FormControlLabel
                    control={<Checkbox color='primary' checked={allowReactions} onClick={e => setAllowReactions(!allowReactions)} />}
                    label='Разрешить реакции'
                    sx={Styles.noSelectSx}
                    disabled={loading}
                />
                <Divider />
                <Box sx={{ width: '100%' }} disabled={loading}>
                    <Box borderColor='divider'>
                        <Tabs value={source} onChange={(e, v) => setSource(v)}>
                            <Tab label='Загрузить файл' />
                            <Tab label='Импортировать видео по ссылке' />
                        </Tabs>
                    </Box>
                    <TabPanel value={source} index={0}>
                        {videoFile && <div style={{ display: 'flex', gap: '0.5em', marginBottom: '0.5em' }}>
                            <TextField
                                fullWidth
                                label='Выбранный файл'
                                defaultValue={videoFile?.name}
                                disabled
                                variant='outlined'
                            />
                            <TextField
                                label='Размер'
                                defaultValue={Common.sizeToHuman(videoFile?.size)}
                                disabled
                                variant='outlined'
                            />
                        </div>}
                        <div style={{ display: 'flex', gap: '0.5em' }}>
                            <Button
                                fullWidth
                                component='label'
                                variant='contained'
                                startIcon={<VideoFileIcon sx={{ height: 40, width: 40 }} />}
                                sx={{ minHeight: 40 }}>
                                Выбрать файл
                                <input
                                    maxLength={MAX_FILE_SIZE}
                                    type='file'
                                    hidden={true}
                                    onChange={onFileSelected}
                                    accept='video/3gpp, video/3gpp2, video/mp4, video/mpeg, video/ogg, video/quicktime, video/vnd.dlna.mpeg-tts, video/webm, video/x-flv, video/x-ivf, video/x-la-asf, video/x-ms-asf, video/x-msvideo, video/x-ms-wm, video/x-ms-wmp, video/x-ms-wmv, video/x-ms-wmx, video/x-ms-wtv, video/x-ms-wvx, video/x-sgi-movie'
                                    style={{
                                        clip: 'rect(0 0 0 0)',
                                        clipPath: 'inset(50%)',
                                        height: 1,
                                        overflow: 'hidden',
                                        position: 'absolute',
                                        bottom: 0,
                                        left: 0,
                                        whiteSpace: 'nowrap',
                                        width: 1,
                                    }} />
                            </Button>
                            {
                                videoFile && <IconButton
                                    sx={{ height: 48, width: 48 }}
                                    onClick={e => onFileSelected()} >
                                    <CloseIcon />
                                </IconButton>
                            }
                        </div>
                    </TabPanel>
                    <TabPanel value={source} index={1}>
                        <TextField
                            fullWidth
                            label='Ссылка на видео или трансляцию'
                            defaultValue={Common.sizeToHuman(videoFile?.size)}
                            error={!Valitation.Video.url(videoUrl)}
                            variant='outlined'
                            value={videoUrl}
                            onChange={e => setVideoUrl(e.target.value)}
                        />
                        <TextField
                            fullWidth
                            required
                            label='Максимальная длительность, если это трансляция'
                            defaultValue={Common.sizeToHuman(videoFile?.size)}
                            error={!Valitation.Video.maxDuration(maxDuration)}
                            variant='outlined'
                            value={maxDuration}
                            onChange={e => setMaxDuration(e.target.value)}
                            sx={{ mt: 2 }}
                        />
                    </TabPanel>
                </Box>
                <Divider />
                <Box sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    alignItems: 'center',
                    gap: 2,
                    ...Styles.noSelectSx
                }}>
                    {
                        !loading && <Button
                            fullWidth
                            variant='outlined'
                            disabled={loading}
                            sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                            onClick={next}
                        >
                            Отмена
                        </Button>
                    }
                    <Button
                        type='submit'
                        fullWidth
                        variant='contained'
                        disabled={loading || !isValid()}
                        sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                    >
                        {loading ? <CircularProgress size={20} /> : 'Начать загрузку'}
                    </Button>
                </Box>
            </Box>
        </Box>
    );
}