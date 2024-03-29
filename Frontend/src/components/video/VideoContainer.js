import { Card, CardActions, CardMedia, FormControl, InputLabel, MenuItem, Select, Typography, Box } from '@mui/material';
import { useState, useRef, useEffect, useContext } from 'react';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import PauseIcon from '@mui/icons-material/Pause';
import VolumeUpIcon from '@mui/icons-material/VolumeUp';
import VolumeOffIcon from '@mui/icons-material/VolumeOff';
import { IconButton, Slider } from '@mui/material';
import Hls from 'hls.js';
import Common from '../../utils/Common';
import DownloadButton from './DownloadButton';
import StateContext from '../../utils/StateContext';
import Rights from '../../utils/Rights';
import EditVideoDialogButton from './EditVideoDialogButton';
import DeleteVideoDialogButton from './DeleteVideoDialogButton';

const RES_AUTO = 'auto';

const PLAYER_TYPE_HLS = 'hls';
const PLAYER_TYPE_MP4 = 'mp4';

const PLAYER_TYPES = [
    Hls.isSupported() ? PLAYER_TYPE_HLS : null,
    PLAYER_TYPE_MP4
].filter(e => e);

/**
 * 
 * @returns {String}
 */
function loadPlayerType() {
    var type = localStorage.getItem('player_type');

    if (PLAYER_TYPES.includes(type)) {
        return type;
    } else {
        return PLAYER_TYPE_HLS;
    }
}

/**
 * 
 * @param {String} type 
 */
function savePlayerType(type) {
    if (PLAYER_TYPES.includes(type)) {
        localStorage.setItem('player_type', type);
    }
}

/**
 * 
 * @returns {Number}
 */
function loadPlayerVolume() {
    var value = localStorage.getItem('player_volume');

    if (value) {
        var num = Number(value);

        if (!isNaN(num)) {
            return num;
        }
    }

    return 100;
}

/**
 * 
 * @param {Number} value 
 */
function savePlayerVolume(value) {
    localStorage.setItem('player_volume', value)
}

const hls = Hls.isSupported() && new Hls({
    xhrSetup: async (xhr, url) => {
        xhr.withCredentials = true;
    }
});

export default function VideoContainer({ video, setVideo, largeMode, setLargeMode }) {
    const { user } = useContext(StateContext);
    const [duration, setDuration] = useState(Common.parseTime(video?.duration));
    const storageKey = `video_${video?.token}`;
    const firstVideoUrl = video.urls[`video${video.resolutions.firstOrDefault()}`];
    const sizes = video.resolutions
        .map(e => {
            return {
                key: `video${e}`,
                name: e
            }
        })
        .filter(e => e.key in video?.downloadSizes)
        .map(e => {
            return {
                ...e,
                size: video?.downloadSizes[e.key]
            }
        });

    const [prevType, setPrevType] = useState();
    const [prevRes, setPrevRes] = useState();

    const videoRef = useRef();
    const [isPlaying, setIsPlaying] = useState(true);
    const [isMuted, setIsMuted] = useState(false);
    const [volume, setVolume] = useState(loadPlayerVolume());
    const [time, setTime] = useState(0);
    const [playerType, setPlayerType] = useState(loadPlayerType());
    const [playerRes, setPlayerRes] = useState(loadResolution());

    const canEditVideo = user?.id && (video?.id && user.id === video.id || Rights.checkAnyRight(user, Rights.VideoGetAll));

    /**
     * 
     * @returns {String[]}
     */
    function getResolutions() {
        return [
            playerType === PLAYER_TYPES[0] && RES_AUTO,
            ...video?.resolutions
        ].filter(e => e);
    }

    function loadResolution() {
        var res = sessionStorage.getItem(`${storageKey}_res`);
        var allRes = getResolutions();

        if (res && allRes.includes(res)) {
            return res;
        } else {
            return allRes[0];
        }
    }

    function saveResolution(res) {
        sessionStorage.setItem(`${storageKey}_res`, res ?? playerRes);
    }

    function onPlayPause() {
        if (videoRef.current.paused) {
            videoRef.current.play();
            setIsPlaying(true);
        } else {
            videoRef.current.pause();
            setIsPlaying(false);
        }
    };

    useEffect(() => {
        setIsPlaying(!videoRef.current.paused);
    }, [videoRef?.current?.paused]);

    function onMute() {
        setIsMuted(!isMuted);
        videoRef.current.muted = !videoRef.current.muted;
    };

    function onVolumeChange(event, newValue) {
        setVolume(newValue);
        savePlayerVolume(newValue);
        videoRef.current.volume = newValue / 100;
    };

    function onTimeUpdate(e) {
        var t = Number(videoRef?.current?.currentTime);

        if (!isNaN(t) && t !== time) {
            sessionStorage.setItem(`${storageKey}_time`, Math.floor(t));
            setTime(t);
        }
    }

    function onPlaybackEnded(e) {
        sessionStorage.removeItem(`${storageKey}_time`);
    }

    function onTimeSeek(e, newValue) {
        if (videoRef?.current) {
            videoRef.current.currentTime = newValue;
        }
    }

    function onVideoLoaded(e) {
        var actualDuration = Math.round(videoRef?.current?.duration);

        if (!isNaN(actualDuration) && actualDuration && actualDuration !== duration) {
            setDuration(actualDuration);
        }

        var value = Number(sessionStorage.getItem(`${storageKey}_time`));

        if (value && !isNaN(value)) {
            onTimeSeek(null, value);
        }

        videoRef.current.volume = loadPlayerVolume() / 100;

        if (videoRef.current.paused && isPlaying) {
            videoRef.current.play();
            setIsPlaying(true);
        }

        videoRef.current.muted = false;
    }

    function onPlayerTypeChange(e, obj) {
        if (obj?.props?.value && playerType !== obj.props.value) {
            setPlayerType(obj.props.value);
            savePlayerType(obj.props.value);
            setPlayerRes(loadResolution());
        }
    }

    function onResolutionChange(e, obj) {
        if (obj?.props?.value && playerRes !== obj.props.value) {
            saveResolution(obj.props.value);
            setPlayerRes(obj.props.value);
        }
    }

    function updateHlsLevel() {
        var levelIndex = getResolutions().indexOf(playerRes);

        if (levelIndex < 0) {
            hls.currentLevel = -1;
        } else {
            hls.currentLevel = levelIndex - 1;
        }
    }

    function toggleLargeMode() {
        setLargeMode && setLargeMode(!largeMode);
    }

    useEffect(() => {
        if (!videoRef?.current) {
            return;
        }

        var wasPlaying = isPlaying;
        var t = time;
        var doUpdate = playerType !== prevType || prevRes !== playerRes;

        // если смена плеера на hls и hls поддерживается
        if (playerType !== prevType && playerType === PLAYER_TYPE_HLS && hls) {
            hls.loadSource(video.urls.hls);
            hls.attachMedia(videoRef.current);
            hls.on(Hls.Events.MANIFEST_PARSED, (ev, data) => {
                updateHlsLevel();
                if (videoRef.current.paused && wasPlaying) {
                    videoRef.current.play();
                }
            });
        }

        // если смена разрешения на hls и hls поддерживается
        if (prevRes !== playerRes && playerType === PLAYER_TYPE_HLS && hls) {
            updateHlsLevel();
        }

        // если смена плеера или разрешения на mp4 или не поддерживается hls
        if (doUpdate && (playerType === PLAYER_TYPE_MP4 || playerType === PLAYER_TYPE_HLS && !hls)) {
            hls?.stopLoad && hls.stopLoad();
            hls?.detachMedia && hls.detachMedia();

            var url = firstVideoUrl;
            var key = `video${playerRes}`;

            if (key in video.urls) {
                url = video.urls[key];
            }
            if (videoRef.current.src != url) {
                videoRef.current.src = url;
            }
        }

        // обновляем плеер
        if (doUpdate) {
            onTimeSeek(null, t);

            if (videoRef.current.paused && wasPlaying) {
                videoRef.current.play();
            }
        }

        // обновление предыдущих значений
        if (prevRes !== playerRes) {
            setPrevRes(playerRes);
        }
        if (prevType !== playerType) {
            setPrevType(playerType);
        }
    }, [playerType, playerRes, videoRef?.current]);

    useEffect(() => {
        const videoElement = document.querySelector('video');

        if (videoElement) {
            videoElement.play(); // Начать воспроизведение видео
        }
    }, [videoRef?.current]);

    return (
        <Card sx={{ marginTop: 3 }}>
            <CardMedia>
                <video
                    ref={videoRef}
                    style={{
                        width: '100%',
                        height: 'auto',
                        minHeight: '300px',
                        maxHeight: largeMode ? '800px' : '600px'
                    }}
                    poster={video.thumbnailUrl}
                    onTimeUpdate={onTimeUpdate}
                    onClick={onPlayPause}
                    onLoadedData={onVideoLoaded}
                    onEnded={onPlaybackEnded}
                    muted
                    autoPlay
                />
            </CardMedia>
            <CardActions>
                <IconButton onClick={onPlayPause}>
                    {isPlaying ? <PauseIcon /> : <PlayArrowIcon />}
                </IconButton>
                <Typography>
                    {Common.timeToHuman(time)}
                </Typography>
                <Slider
                    min={0}
                    max={duration}
                    value={time}
                    valueLabelDisplay='auto'
                    size='small'
                    valueLabelFormat={Common.timeToHuman}
                    onChange={onTimeSeek}
                    style={{
                        flex: 1,
                        margin: '0 10px',
                    }}
                />
                <Typography>
                    {Common.timeToHuman(time - duration)}
                </Typography>
                <Slider
                    min={0}
                    max={100}
                    value={volume}
                    onChange={onVolumeChange}
                    valueLabelDisplay='auto'
                    size='small'
                    style={{
                        width: 100,
                        margin: '0 10px',
                    }}
                />
                <IconButton onClick={onMute}>
                    {isMuted ? <VolumeOffIcon /> : <VolumeUpIcon />}
                </IconButton>
            </CardActions>
            <CardActions>
                <Box sx={{ display: 'flex', gap: '16px' }}>
                    <FormControl sx={{ minWidth: 100 }}>
                        <InputLabel htmlFor='playertype'>Плеер</InputLabel>
                        <Select
                            id='playertype'
                            label='Player type'
                            value={playerType}
                            onChange={onPlayerTypeChange}
                        >
                            {PLAYER_TYPES.map((e, i) => <MenuItem value={e} key={i}>{e.toUpperCase()}</MenuItem>)}
                        </Select>
                    </FormControl>
                    <FormControl sx={{ minWidth: 100 }}>
                        <InputLabel htmlFor='playerres'>Разрешение</InputLabel>
                        <Select
                            id='playerres'
                            label='Resolution'
                            value={getResolutions().some(e => e === playerRes) ? playerRes : getResolutions().firstOrDefault()}
                            onChange={onResolutionChange}
                        >
                            {getResolutions().map((e, i) => <MenuItem value={e} key={i} >
                                {e}{playerType === PLAYER_TYPE_HLS && hls.currentLevel + 1 === i ? ' \u2022' : ''}
                            </MenuItem>)}
                        </Select>
                    </FormControl>
                    <DownloadButton token={video?.token} sizes={sizes} />
                    {canEditVideo && <EditVideoDialogButton video={video} setVideo={setVideo} />}
                    {canEditVideo && <DeleteVideoDialogButton video={video} setVideo={setVideo} />}
                </Box>
            </CardActions>
        </Card>
    );
}