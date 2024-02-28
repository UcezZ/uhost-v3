import React, { useState, useRef } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import { IconButton, Slider } from '@material-ui/core';
import PlayArrowIcon from '@material-ui/icons/PlayArrow';
import PauseIcon from '@material-ui/icons/Pause';
import VolumeUpIcon from '@material-ui/icons/VolumeUp';
import VolumeOffIcon from '@material-ui/icons/VolumeOff';

const useStyles = makeStyles({
    video: {
        maxWidth: '100%',
        height: 'auto',
    },
    controls: {
        display: 'flex',
        alignItems: 'center',
    },
    slider: {
        width: 200,
        margin: '0 10px',
    }
});

export default function VideoPlayer({ src }) {
    const classes = useStyles();
    const videoRef = useRef();
    const [isPlaying, setIsPlaying] = useState(false);
    const [isMuted, setIsMuted] = useState(false);
    const [volume, setVolume] = useState(100);

    const handlePlayPause = () => {
        if (videoRef.current.paused) {
            videoRef.current.play();
            setIsPlaying(true);
        } else {
            videoRef.current.pause();
            setIsPlaying(false);
        }
    };

    const handleVolume = () => {
        setIsMuted(!isMuted);
        videoRef.current.muted = !videoRef.current.muted;
    };

    const handleVolumeChange = (event, newValue) => {
        setVolume(newValue);
        videoRef.current.volume = newValue / 100;
    };

    return (
        <div>
            <video
                ref={videoRef}
                className={classes.video}
                src={src}
            />
            <div className={classes.controls}>
                <IconButton onClick={handlePlayPause}>
                    {isPlaying ? <PauseIcon /> : <PlayArrowIcon />}
                </IconButton>
                <Slider
                    className={classes.slider}
                    value={volume}
                    onChange={handleVolumeChange}
                    aria-labelledby="volume-slider"
                />
                <IconButton onClick={handleVolume}>
                    {isMuted ? <VolumeOffIcon /> : <VolumeUpIcon />}
                </IconButton>
            </div>
        </div>
    );
};
