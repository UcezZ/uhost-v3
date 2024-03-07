import { Card, CardActions, CardMedia, FormControl, InputLabel, MenuItem, Select, Typography } from '@mui/material';
import { useState, useRef, useEffect } from 'react';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import PauseIcon from '@mui/icons-material/Pause';
import VolumeUpIcon from '@mui/icons-material/VolumeUp';
import VolumeOffIcon from '@mui/icons-material/VolumeOff';
import { IconButton, Slider } from '@mui/material';
import Hls from 'hls.js';
import Common from '../utils/Common';

const resolutionAuto = 'auto';

const typeHls = 'hls';
const typeMp4 = 'mp4';

const playerTypes = [
    Hls.isSupported() && typeHls,
    typeMp4
].filter(e => e);

/**
 * 
 * @returns {String}
 */
function loadPlayerType() {
    var type = localStorage.getItem('player_type');

    if (playerTypes.includes(type)) {
        return type;
    } else {
        return typeHls;
    }
}

/**
 * 
 * @param {String} type 
 */
function savePlayerType(type) {
    if (playerTypes.includes(type)) {
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

export default function VideoContainer({ video }) {
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
            playerType === playerTypes[0] && resolutionAuto,
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

    useEffect(() => {
        console.log(1);
        if (!videoRef?.current) {
            return;
        }

        var wasPlaying = isPlaying;
        var t = time;
        var doUpdate = playerType !== prevType || prevRes !== playerRes;

        // если смена плеера на hls и hls поддерживается
        if (playerType !== prevType && playerType === typeHls && hls) {
            hls.loadSource(video.urls.hls);
            hls.attachMedia(videoRef.current);
            hls.on(Hls.Events.MANIFEST_PARSED, (ev, data) => {
                if (videoRef.current.paused && wasPlaying) {
                    videoRef.current.play();
                }
            });
        }

        // если смена разрешения на hls и hls поддерживается
        if (prevRes !== playerRes && playerType === typeHls && hls) {
            var levelIndex = getResolutions().indexOf(playerRes);

            if (levelIndex < 0) {
                hls.currentLevel = -1;
            } else {
                hls.currentLevel = levelIndex - 1;
            }
        }

        // если смена плеера или разрешения на mp4 или не поддерживается hls
        if (doUpdate && (playerType === typeMp4 || playerType === typeHls && !hls)) {
            hls?.stopLoad();
            hls?.detachMedia();
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
                <FormControl sx={{ minWidth: 100 }}>
                    <InputLabel htmlFor='playertype'>Player type</InputLabel>
                    <Select
                        id='playertype'
                        label='Player type'
                        value={playerType}
                        onChange={onPlayerTypeChange}
                    >
                        {playerTypes.map((e, i) => <MenuItem value={e} key={i}>{e.toUpperCase()}</MenuItem>)}
                    </Select>
                </FormControl>
                <FormControl sx={{ minWidth: 100 }}>
                    <InputLabel htmlFor='playerres'>Resolution</InputLabel>
                    <Select
                        id='playerres'
                        label='Resolution'
                        value={getResolutions().any(e => e === playerRes) ? playerRes : getResolutions().firstOrDefault()}
                        onChange={onResolutionChange}
                    >
                        {getResolutions().map((e, i) => <MenuItem value={e} key={i} >
                            {e}{playerType === typeHls && hls.currentLevel + 1 === i ? ' \u2022' : ''}
                        </MenuItem>)}
                    </Select>
                </FormControl>
            </CardActions>
        </Card>
    );
}