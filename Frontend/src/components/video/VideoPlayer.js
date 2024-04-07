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
import VideoDummy from './VideoDummy';
import config from '../../config.json';

const RES_AUTO = 'auto';
const RES_WEBM = 'videoWebm';

const IS_HLS_SUPPORTED = Common.isHlsSupported();
const IS_MP4_SUPPORTED = Common.isMp4Supported();

const PLAYER_TYPE_HLS = 'hls';
const PLAYER_TYPE_MP4 = 'mp4';
const PLAYER_TYPE_WEBM = 'webm';

const MP4_RESOLUTIONS = [
    'video240p',
    'video480p',
    'video720p',
    'video1080p'
];

const HLS_RESOLUTIONS = [
    RES_AUTO,
    ...MP4_RESOLUTIONS
];

const WEBM_RESOLUTIONS = [
    RES_WEBM
];

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

export default function VideoPlayer({ video, largeMode }) {
    const { t } = useTranslation();
    const [duration, setDuration] = useState(Common.parseTime(video?.duration));
    const storageKey = `video_${video?.token}`;

    const [prevType, setPrevType] = useState();
    const [prevRes, setPrevRes] = useState();

    const videoRef = useRef();
    const [isPlaying, setIsPlaying] = useState(true);
    const [isMuted, setIsMuted] = useState(false);
    const [volume, setVolume] = useState(loadPlayerVolume());
    const [time, setTime] = useState(0);
    const [playerType, setPlayerType] = useState(loadPlayerType());
    const [playerRes, setPlayerRes] = useState(loadResolution());

    const hls = IS_HLS_SUPPORTED ? new Hls({
        xhrSetup: function (xhr, url) {
            xhr.withCredentials = true;
            xhr.setRequestHeader('Access-Token', video?.accessToken);
        },
        ...config.hlsConfig
    }) : null;

    function turnOffHls() {
        if (IS_HLS_SUPPORTED) {
            try {
                hls?.stopLoad && hls.stopLoad();
                hls?.destroy && hls.destroy();
            } catch { }
        }
    }

    function getPlayerTypes() {
        return [
            IS_HLS_SUPPORTED && HLS_RESOLUTIONS.some(e => video.resolutions.includes(e)) ? PLAYER_TYPE_HLS : null,
            IS_MP4_SUPPORTED && MP4_RESOLUTIONS.some(e => video.resolutions.includes(e)) ? PLAYER_TYPE_MP4 : null,
            WEBM_RESOLUTIONS.some(e => video.resolutions.includes(e)) ? PLAYER_TYPE_WEBM : null
        ].filter(e => e);
    }

    /**
     * 
     * @returns {String}
     */
    function loadPlayerType() {
        var type = localStorage.getItem('player_type');
        var allTypes = getPlayerTypes();

        if (allTypes.includes(type)) {
            return type;
        } else {
            return allTypes[0];
        }
    }

    /**
     * 
     * @param {String} type 
     */
    function savePlayerType(type) {
        if (getPlayerTypes().includes(type)) {
            localStorage.setItem('player_type', type);
        }
    }

    const NO_PLAYER = getPlayerTypes().length === 0;

    function getResolutions() {
        switch (playerType) {
            case PLAYER_TYPE_HLS:
                return HLS_RESOLUTIONS.filter(e => e === RES_AUTO || video.resolutions.includes(e));
            case PLAYER_TYPE_MP4:
                return MP4_RESOLUTIONS.filter(e => video.resolutions.includes(e));
            case PLAYER_TYPE_WEBM:
                return WEBM_RESOLUTIONS;
        }
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
        try {
            setIsPlaying(!videoRef.current.paused);
        }
        catch { }
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
        var levelIndex = HLS_RESOLUTIONS.indexOf(playerRes);

        if (levelIndex < 0) {
            hls.currentLevel = -1;
        } else {
            hls.currentLevel = levelIndex - 1;
        }
    }

    function updateVideoSource() {
        var allRes = getResolutions();

        var res = playerRes;

        if (!allRes.includes(res)) {
            console.log(`${res} not found, ${playerType}`);
            res = allRes[0];
        }
        if (res in video.urls) {
            var url = video.urls[res];

            if (url && videoRef?.current?.src?.startsWith && !videoRef.current.src.startsWith(url)) {
                videoRef.current.src = `${url}?access_token=${video?.accessToken}`;
            } else {
                console.log(`source not updated, ${url}`);
            }
        } else {
            console.log(`${res} not found in video`);
        }
    }

    useEffect(() => {
        if (!videoRef?.current) {
            return;
        }

        var wasPlaying = isPlaying;
        var t = time;
        var doUpdate = playerType !== prevType || prevRes !== playerRes;

        if (!doUpdate) {
            return;
        }

        if (playerType !== prevType) {
            // выключаем HLS
            if (prevType === PLAYER_TYPE_HLS) {
                turnOffHls();
            }

            switch (playerType) {
                case PLAYER_TYPE_HLS:
                    hls.loadSource(video.urls.hls);
                    hls.attachMedia(videoRef.current);
                    hls.on(Hls.Events.MANIFEST_PARSED, (ev, data) => {
                        updateHlsLevel();
                        if (videoRef.current.paused && wasPlaying) {
                            videoRef.current.play();
                        }
                    });
                    break;
                default:
                    updateVideoSource();
                    break;
            }
        } else {
            switch (playerType) {
                case PLAYER_TYPE_HLS:
                    updateHlsLevel();
                    break;
                default:
                    updateVideoSource();
                    break;
            }
        }

        // обновляем плеер
        if (doUpdate) {
            onTimeSeek(null, t);

            if (videoRef.current.paused && wasPlaying) {
                try {
                    videoRef.current.play();
                } catch { }
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

    // останавливаем HLS при размонтировании компонента
    useEffect(() => turnOffHls, []);

    if (NO_PLAYER) {
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
                    <VideoDummy video={video} />
                </CardMedia>
            </div>
        );
    }

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
                    onError={console.log}
                    onContextMenu={e => false}
                    onContextMenuCapture={e => false}
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
                            label={t('video.player')}
                            value={playerType}
                            onChange={onPlayerTypeChange}
                        >
                            {getPlayerTypes().map((e, i) => <MenuItem value={e} key={i}>{t(`video.player.${e}`)}</MenuItem>)}
                        </Select>
                    </FormControl>

                    {
                        playerType !== PLAYER_TYPE_WEBM && <FormControl sx={{ minWidth: 100 }}>
                            <InputLabel htmlFor='playerres'>{t('video.resolution')}</InputLabel>
                            <Select
                                id='playerres'
                                label={t('video.resolution')}
                                value={getResolutions().some(e => e === playerRes) ? playerRes : getResolutions().firstOrDefault()}
                                onChange={onResolutionChange}
                            >
                                {
                                    getResolutions().map((e, i) => <MenuItem value={e} key={i} >
                                        {t(`video.resolution.${e}`)}{playerType === PLAYER_TYPE_HLS && hls.currentLevel + 1 === i ? ' \u2022' : ''}
                                    </MenuItem>)
                                }
                            </Select>
                        </FormControl>
                    }
                </Box>
            </CardActions>
        </div>
    );
}