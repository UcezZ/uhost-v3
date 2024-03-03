import { Card, CardActions, CardMedia, InputLabel, MenuItem, Select, Typography } from '@mui/material';
import { useState, useRef } from 'react';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import PauseIcon from '@mui/icons-material/Pause';
import VolumeUpIcon from '@mui/icons-material/VolumeUp';
import VolumeOffIcon from '@mui/icons-material/VolumeOff';
import { IconButton, Slider } from '@mui/material';
import Hls from 'hls.js';
import Common from '../utils/Common';

const playerTypes = ['hls', 'mp4'];

export default function VideoContainer({ video }) {
    const videoRef = useRef();
    const [isPlaying, setIsPlaying] = useState(false);
    const [isMuted, setIsMuted] = useState(false);
    const [volume, setVolume] = useState(100);
    const [time, setTime] = useState(0);
    const [source, setSource] = useState(video?.urls[Object.keys(video.urls).filter(e => e?.startsWith && e.startsWith('video'))[0]]);
    const [playerType, setPlayerType] = useState(playerTypes[0]);
    const duration = Common.parseTime(video?.duration);
    const storageKey = `video_${video?.token}`;

    function onPlayPause() {
        if (videoRef.current.paused) {
            videoRef.current.play();
            setIsPlaying(true);
        } else {
            videoRef.current.pause();
            setIsPlaying(false);
        }
    };

    function onMute() {
        setIsMuted(!isMuted);
        videoRef.current.muted = !videoRef.current.muted;
    };

    function onVolumeChange(event, newValue) {
        setVolume(newValue);
        videoRef.current.volume = newValue / 100;
    };

    function onTimeUpdate(e) {
        var t = Number(videoRef?.current?.currentTime);

        if (!isNaN(t) && t !== time) {
            sessionStorage.setItem(`${storageKey}_time`, Math.floor(t));
            setTime(t);
        }
    }

    function onTimeSeek(e, newValue) {
        if (videoRef?.current) {
            videoRef.current.currentTime = newValue;
        }
    }

    function onVideoLoaded(e) {
        var value = Number(sessionStorage.getItem(`${storageKey}_time`));

        if (value && !isNaN(value)) {
            onTimeSeek(null, value);
        }

        onPlayPause();
        videoRef.current.muted = false;
    }

    var hls = new Hls();

    return (
        <Card sx={{ marginTop: 3 }}>
            <CardMedia>
                <video
                    ref={videoRef}
                    src={source}
                    style={{
                        width: '100%',
                        height: 'auto',
                    }}
                    onTimeUpdate={onTimeUpdate}
                    onClick={onPlayPause}
                    onLoadedData={onVideoLoaded}
                    muted
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
            {/* <CardActions>
                <InputLabel id='playertype-label'>Player type</InputLabel>
                <Select
                    labelId='playertype-label'
                    label='Player type'
                >
                    <MenuItem value='hls'>HLS</MenuItem>
                    <MenuItem value='mp4'>MP4</MenuItem>
                </Select>
                <InputLabel id='playerres-label'>Resolution</InputLabel>
                <Select
                    labelId='playerres'
                    label='Resolution'>
                    {video.resolutions.map(e => <MenuItem value={e}>{e}</MenuItem>)}
                </Select>
            </CardActions> */}
        </Card>
    );
}