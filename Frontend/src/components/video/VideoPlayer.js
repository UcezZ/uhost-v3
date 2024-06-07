import { CardActions, CardMedia, FormControl, InputLabel, MenuItem, Select, Typography, Box, useMediaQuery } from '@mui/material';
import { useState, useRef, useEffect, useContext } from 'react';
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
import * as Sentry from '@sentry/browser';
import FullscreenIcon from '@mui/icons-material/Fullscreen';
import StateContext from '../../utils/StateContext';

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
    try {
        var value = localStorage.getItem('player_volume');

        if (value) {
            var num = Number(value);

            if (!isNaN(num)) {
                return num;
            }
        }
    } catch (err) {
        Sentry.withScope(scope => {
            scope.setExtra('msg', { message: 'failed to get player_volume from localStorage in VideoPlayer' });
            Sentry.captureException(err);
        });
    }

    return 100;
}

/**
 * 
 * @param {Number} value 
 */
function savePlayerVolume(value) {
    try {
        localStorage.setItem('player_volume', value);
    } catch (err) {
        Sentry.withScope(scope => {
            scope.setExtra('msg', { message: 'failed to set player_volume to localStorage in VideoPlayer' });
            Sentry.captureException(err);
        });
    }
}

var setHeaderFunc = (xhr) => false;

const hls = IS_HLS_SUPPORTED ? new Hls({
    xhrSetup: function (xhr, url) {
        xhr.withCredentials = true;
        setHeaderFunc && setHeaderFunc(xhr);
    },
    ...config.hlsConfig
}) : null;

export default function VideoPlayer({ video, largeMode }) {
    const { t } = useTranslation();
    const [duration, setDuration] = useState(Common.parseTime(video?.duration));
    const storageKey = `video_${video?.token}`;

    const [prevType, setPrevType] = useState();
    const [prevRes, setPrevRes] = useState();

    const videoRef = useRef();
    const [isPlaying, setIsPlaying] = useState(true);
    const [isMuted, setIsMuted] = useState(true);
    const [volume, setVolume] = useState(loadPlayerVolume());
    const [time, setTime] = useState(0);
    const [playerType, setPlayerType] = useState(loadPlayerType());
    const [playerRes, setPlayerRes] = useState(loadResolution());

    const isNarrowScreen = useMediaQuery('(max-width:600px)');

    if (IS_HLS_SUPPORTED) {
        setHeaderFunc = xhr => xhr.setRequestHeader('Access-Token', video?.accessToken);
    }

    function turnOffHls() {
        if (IS_HLS_SUPPORTED) {
            try {
                hls?.stopLoad && hls.stopLoad();
                //hls?.destroy && hls.destroy();
                console.log('hls stopped');
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
        var allTypes = getPlayerTypes();

        try {
            var type = localStorage.getItem('player_type');

            if (allTypes.includes(type)) {
                return type;
            }
        } catch (err) {
            Sentry.withScope(scope => {
                scope.setExtra('msg', { message: 'failed to get player_type from localStorage in VideoPlayer' });
                Sentry.captureException(err);
            });
        }

        return allTypes[0];
    }

    /**
     * 
     * @param {String} type 
     */
    function savePlayerType(type) {
        if (getPlayerTypes().includes(type)) {
            try {
                localStorage.setItem('player_type', type);
            } catch (err) {
                Sentry.withScope(scope => {
                    scope.setExtra('msg', { message: 'failed to set player_type to localStorage in VideoPlayer' });
                    Sentry.captureException(err);
                });
            }
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
                return WEBM_RESOLUTIONS.filter(e => video.resolutions.includes(e));
            default:
                return [];
        }
    }

    function loadResolution() {
        var allRes = getResolutions();

        try {
            var res = localStorage.getItem(`video_res`);

            if (res && allRes.includes(res)) {
                return res;
            }
        } catch (err) {
            Sentry.withScope(scope => {
                scope.setExtra('msg', { message: 'failed to get video_res from localStorage in VideoPlayer' });
                Sentry.captureException(err);
            });
        }

        return allRes[0];
    }

    function saveResolution(res) {
        try {
            localStorage.setItem(`video_res`, res ?? playerRes);
        } catch (err) {
            Sentry.withScope(scope => {
                scope.setExtra('msg', { message: 'failed to set video_res to localStorage in VideoPlayer' });
                Sentry.captureException(err);
            });
        }
    }

    async function onPlayPause() {
        if (videoRef.current.paused) {
            await videoRef.current.play();
            setIsPlaying(true);
        } else {
            await videoRef.current.pause();
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
        if (videoRef?.current) {
            // videoRef.current.muted = !videoRef.current.muted;
            // setIsMuted(videoRef.current.muted);
            setIsMuted(!isMuted);
        }
    };

    function onVolumeChange(event, newValue) {
        setVolume(newValue);
        savePlayerVolume(newValue);
        videoRef.current.volume = newValue / 100;
    };

    function onTimeUpdate(e) {
        var t = Number(videoRef?.current?.currentTime);

        if (!isNaN(t) && t !== time) {
            try {
                sessionStorage.setItem(`${storageKey}_time`, Math.floor(t));
            }
            catch { }
            setTime(t);
        }
    }

    function onPlaybackEnded(e) {
        try {
            sessionStorage.removeItem(`${storageKey}_time`);
        } catch { }
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

        if (isNarrowScreen) {
            videoRef.current.volume = 1;
        } else {
            videoRef.current.volume = loadPlayerVolume() / 100;
        }

        if (videoRef.current.paused) {
            try {
                videoRef.current.play();
                setIsPlaying(true);
            } catch { }
        }

        setIsMuted(false);
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
        var res = HLS_RESOLUTIONS.filter(e => e === RES_AUTO || video.resolutions.includes(e));
        var levelIndex = res.indexOf(playerRes);

        console.log(res.indexOf(playerRes), res, playerRes);

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

    function onFullScreenToggle() {
        if (document.fullscreenElement) {
            document.exitFullscreen();
        } else {
            videoRef?.current?.requestFullscreen && videoRef.current.requestFullscreen();
        }
    }

    // когда скрыт слайдер громкости громкость определяется по муту
    useEffect(() => {
        if (!videoRef?.current) {
            return;
        }

        if (isNarrowScreen) {
            videoRef.current.volume = 1;
        } else {
            videoRef.current.volume = volume / 100;
        }
    }, [isNarrowScreen]);

    // переключение видео
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
                            try {
                                videoRef.current.play();
                            } catch { }
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

    // останавливаем HLS при размонтировании компонента
    useEffect(() => {
        return () => {
            if (!videoRef?.current) {
                turnOffHls();
            }
        }
    }, []);

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
                    onDoubleClick={onFullScreenToggle}
                    onLoadedData={onVideoLoaded}
                    onEnded={onPlaybackEnded}
                    onError={console.log}
                    onContextMenu={e => false}
                    onContextMenuCapture={e => false}
                    loop={video?.loopPlayback === true}
                    muted={isMuted}
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
                {
                    !isNarrowScreen && <Slider
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
                }
                <IconButton onClick={onMute}>
                    {isMuted ? <VolumeOffIcon /> : <VolumeUpIcon />}
                </IconButton>
                <IconButton onClick={onFullScreenToggle}>
                    <FullscreenIcon />
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