import { CardActions, CardMedia, FormControl, InputLabel, MenuItem, Select, Typography, Box } from '@mui/material';
import { useState, useRef, useEffect } from 'react';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import PauseIcon from '@mui/icons-material/Pause';
import VolumeUpIcon from '@mui/icons-material/VolumeUp';
import VolumeOffIcon from '@mui/icons-material/VolumeOff';
import { IconButton, Slider } from '@mui/material';
import Hls from 'hls.js';
import Common from '../../utils/Common';
import { useTranslation } from 'react-i18next';

const RES_AUTO = 'auto';

const IS_HLS_SUPPORTED = Hls.isSupported();

const PLAYER_TYPE_HLS = 'hls';
const PLAYER_TYPE_MP4 = 'mp4';

const PLAYER_TYPES = [
    IS_HLS_SUPPORTED ? PLAYER_TYPE_HLS : null,
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

export default function VideoPlayer({ video, largeMode }) {
    const { t } = useTranslation();
    const [duration, setDuration] = useState(Common.parseTime(video?.duration));
    const storageKey = `video_${video?.token}`;
    const firstVideoUrl = video.urls[`video${video.resolutions.firstOrDefault()}`];

    const [prevType, setPrevType] = useState();
    const [prevRes, setPrevRes] = useState();

    const videoRef = useRef();
    const [isPlaying, setIsPlaying] = useState(true);
    const [isMuted, setIsMuted] = useState(false);
    const [volume, setVolume] = useState(loadPlayerVolume());
    const [time, setTime] = useState(0);
    const [playerType, setPlayerType] = useState(loadPlayerType());
    const [playerRes, setPlayerRes] = useState(loadResolution());

    /**
     * 
     * @returns {String[]}
     */
    function getResolutions() {
        return [
            playerType === PLAYER_TYPES[0] && IS_HLS_SUPPORTED ? RES_AUTO : null,
            ...video?.resolutions
        ].filter(e => e);
    }

    function loadResolution() {
        var res = localStorage.getItem(`video_res`);
        var allRes = getResolutions();

        if (res && allRes.includes(res)) {
            return res;
        } else {
            return allRes[0];
        }
    }

    function saveResolution(res) {
        localStorage.setItem(`video_res`, res ?? playerRes);
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

    useEffect(() => {
        if (!videoRef?.current) {
            return;
        }

        var wasPlaying = isPlaying;
        var t = time;
        var doUpdate = playerType !== prevType || prevRes !== playerRes;

        // если смена плеера на hls и hls поддерживается
        if (playerType !== prevType && playerType === PLAYER_TYPE_HLS && IS_HLS_SUPPORTED) {
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
        if ((prevRes !== playerRes || playerType === PLAYER_TYPE_HLS && prevType !== PLAYER_TYPE_HLS) && IS_HLS_SUPPORTED) {
            updateHlsLevel();
        }

        // если смена плеера или разрешения на mp4 или не поддерживается hls
        if (doUpdate && (playerType === PLAYER_TYPE_MP4 || playerType === PLAYER_TYPE_HLS && !IS_HLS_SUPPORTED)) {
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
            try {
                videoElement.play();
            }
            catch { }
        }
    }, [videoRef?.current]);

    return (
        <div>
            <CardMedia
                sx={{
                    padding: 0,
                    margin: 0,
                    backgroundColor: '#000',
                    minHeight: '300px',
                    display: 'flex',
                    maxHeight: largeMode ? '100%' : '600px',
                }}>
                <video
                    ref={videoRef}
                    style={{
                        flex: 1,
                        padding: 0,
                        margin: 0,
                        maxWidth: '100%'
                    }}
                    poster={video.thumbnailUrl}
                    onTimeUpdate={onTimeUpdate}
                    onClick={onPlayPause}
                    onLoadedData={onVideoLoaded}
                    onEnded={onPlaybackEnded}
                    onContextMenu={e => false}
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
                        <InputLabel htmlFor='playertype'>{t('video.player')}</InputLabel>
                        <Select
                            id='playertype'
                            label='Player type'
                            value={playerType}
                            onChange={onPlayerTypeChange}
                        >
                            {PLAYER_TYPES.map((e, i) => <MenuItem value={e} key={i}>{t(`video.player.${e}`)}</MenuItem>)}
                        </Select>
                    </FormControl>
                    <FormControl sx={{ minWidth: 100 }}>
                        <InputLabel htmlFor='playerres'>{t('video.resolution')}</InputLabel>
                        <Select
                            id='playerres'
                            label='Resolution'
                            value={getResolutions().some(e => e === playerRes) ? playerRes : getResolutions().firstOrDefault()}
                            onChange={onResolutionChange}
                        >
                            {getResolutions().map((e, i) => <MenuItem value={e} key={i} >
                                {t(`video.resolution.${e}`)}{playerType === PLAYER_TYPE_HLS && hls.currentLevel + 1 === i ? ' \u2022' : ''}
                            </MenuItem>)}
                        </Select>
                    </FormControl>
                </Box>
            </CardActions>
        </div>
    );
}