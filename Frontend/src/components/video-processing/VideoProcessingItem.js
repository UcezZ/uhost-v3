import { Accordion, AccordionDetails, AccordionSummary, Box, Grid, LinearProgress, Typography, useMediaQuery } from '@mui/material';
import Image from '../Image';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { useState, useEffect } from 'react';
import { green, grey, red, yellow } from '@mui/material/colors';
import Common from '../../utils/Common';
import VideoEndpoint from '../../api/VideoEndpoint';
import Styles from '../../ui/Styles';
import LoadingBox from '../LoadingBox';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import config from '../../config.json';

const REFRESH_THRESHOLD = 1500;

const STATE_COMPLETED = 'Completed';
const STATE_PROCESSING = 'Processing';
const STATE_FAILED = 'Failed';
const STATE_PENDING = 'Pending';

const COLOR_ALPHA = '#0004';
const COLOR_PENDING = Common.combineColors(grey[800], COLOR_ALPHA);
const COLOR_PROCESSING = Common.combineColors(yellow[800], COLOR_ALPHA);
const COLOR_COMPLETED = Common.combineColors(green[800], COLOR_ALPHA);
const COLOR_FAILED = Common.combineColors(red[800], COLOR_ALPHA);

function colorByState(state) {
    switch (state) {
        case STATE_COMPLETED:
            return COLOR_COMPLETED;
        case STATE_PROCESSING:
            return COLOR_PROCESSING;
        case STATE_FAILED:
            return COLOR_FAILED;
        default:
            return COLOR_PENDING;
    }
}

function percentageWithState(percentage, state) {
    switch (state) {
        case STATE_COMPLETED:
            return 100;
        case STATE_PROCESSING:
            return percentage;
        default:
            return 0;
    }
}

function transformData(data) {
    if (!data?.states) {
        return [];
    }

    var keys = Object.keys(data.states);

    if (!keys?.length) {
        return [];
    }

    return keys.map(e => {
        return {
            key: e.toLowerCase(),
            state: data.states[e],
            percentage: percentageWithState(e in data?.progresses ? data.progresses[e] : 0, data.states[e]),
            isWaiting: data.states[e] === STATE_PENDING
        };
    });
}

export default function VideoProcessingItem({ video, expanded }) {
    const { t } = useTranslation();
    const [isExpanded, setIsExpanded] = useState(Boolean(expanded) ?? false);
    const [data, setData] = useState([]);
    const [lastUpdate, setLastUpdate] = useState(new Date().getTime());
    const [allCompleded, setAllCompleded] = useState(false);
    const isNarrowScreen = useMediaQuery('(max-width:700px)');

    async function onUpdate() {
        await VideoEndpoint.getProgress(video?.token)
            .then(e => {
                if (e?.data?.success && e?.data?.result?.states && e?.data?.result?.progresses) {
                    var data = transformData(e.data.result);
                    setData(data);
                    if (data.every(e => e?.state === STATE_COMPLETED)) {
                        setAllCompleded(true);
                        video.state = STATE_COMPLETED;
                    } else if (data.some(e => e?.state === STATE_PROCESSING)) {
                        video.state = STATE_PROCESSING;
                    } else if (data.some(e => e?.state === STATE_FAILED)) {
                        video.state = STATE_FAILED;
                    }
                } else {
                    console.log(e);
                }
            }).catch(console.error);
        await Common.sleep(REFRESH_THRESHOLD);
        setLastUpdate(new Date().getTime())
    }

    useEffect(() => {
        if (isExpanded && !allCompleded) {
            onUpdate();
        }
    }, [lastUpdate, isExpanded]);

    return (
        <Accordion
            expanded={isExpanded}
            onChange={(e, v) => setIsExpanded(v)}
            sx={{
                backgroundColor: colorByState(video?.state),
                mt: 1,
                mb: 1,
                borderRadius: 2
            }} >
            <AccordionSummary expandIcon={<ExpandMoreIcon />} >
                {isNarrowScreen ?
                    <Grid container spacing={2} >
                        <Grid item xs={12} >
                            <Link to={`${config.webroot}/video/${video?.token}`}>
                                <Image
                                    src={video?.thumbnailUrl}
                                    height='12em'
                                    sx={{
                                        borderRadius: 4
                                    }}
                                />
                            </Link>
                        </Grid>
                        <Grid
                            item
                            alignItems='center'
                            xs={6}
                            sx={{
                                pb: 3,
                                pt: 1
                            }} >
                            <Typography
                                variant='h6'
                                sx={{
                                    fontSize: 16,
                                    fontWeight: 700
                                }}
                                noWrap>
                                {t(video?.state?.length ? `video.processing.state.${video.state.toLowerCase()}` : 'N/A')}
                            </Typography>
                            <Typography
                                noWrap>
                                {video?.name ?? 'N/A'}
                            </Typography>
                        </Grid>
                    </Grid>
                    :
                    <Grid container spacing={2} >
                        <Grid item  >
                            <Link to={`${config.webroot}/video/${video?.token}`}>
                                <Image
                                    src={video?.thumbnailUrl}
                                    height='5.5em'
                                    width='9em'
                                    sx={{
                                        borderRadius: 4
                                    }} />
                            </Link>
                        </Grid>
                        <Grid
                            item
                            alignItems='center'
                            xs={6}
                            sx={{
                                pb: 3,
                                pt: 1
                            }} >
                            <Typography
                                variant='h6'
                                sx={{
                                    fontSize: 16,
                                    fontWeight: 700
                                }}
                                noWrap>
                                {t(video?.state?.length ? `video.processing.state.${video.state.toLowerCase()}` : 'N/A')}
                            </Typography>
                            <Typography
                                noWrap>
                                {video?.name ?? 'N/A'}
                            </Typography>
                        </Grid>
                    </Grid>
                }
            </AccordionSummary>
            <AccordionDetails
                sx={{
                    p: 1
                }} >
                {
                    data?.length > 0
                        ? data.map((e, i) => isNarrowScreen
                            ? <Grid
                                container
                                backgroundColor={colorByState(e?.state)}
                                mt={1}
                                borderRadius={1}
                                alignItems='center'
                                textAlign='center'
                                key={i}>
                                <Grid item pt={1} xs={4}>
                                    <Typography>
                                        {t(e?.key?.length ? `video.processing.type.${e.key}` : 'N/A')}
                                    </Typography>
                                </Grid>
                                <Grid item pt={1} xs={4}>
                                    <Typography>
                                        {t(e?.state?.length ? `video.processing.state.${e.state.toLowerCase()}` : 'N/A')}
                                    </Typography>
                                </Grid>
                                <Grid item pt={1} xs={4}>
                                    <Typography>{Math.round(e.percentage)}%</Typography>
                                </Grid>
                                <Grid item flex={1} p={1} xs={12} >
                                    <LinearProgress
                                        value={e.percentage}
                                        sx={{
                                            flex: 1,
                                            height: 16,
                                            borderRadius: 16
                                        }}
                                        variant={e.isWaiting ? 'indeterminate' : 'determinate'} />
                                </Grid>
                            </Grid>
                            : <Grid
                                key={i}
                                container
                                alignItems='center'
                                textAlign='center'
                                backgroundColor={colorByState(e?.state)}
                                borderRadius={1}
                                mt={1}
                            >
                                <Grid item p={2} xs={2.5} >
                                    <Typography>
                                        {t(e?.key?.length ? `video.processing.type.${e.key}` : 'N/A')}
                                    </Typography>
                                </Grid>
                                <Grid item xs={2}>
                                    <Typography>
                                        {t(e?.state?.length ? `video.processing.state.${e.state.toLowerCase()}` : 'N/A')}
                                    </Typography>
                                </Grid>
                                <Grid item flex={1} p={1} >
                                    <LinearProgress
                                        value={e.percentage}
                                        sx={{
                                            flex: 1,
                                            height: 16,
                                            borderRadius: 16
                                        }}
                                        variant={e.isWaiting ? 'indeterminate' : 'determinate'} />
                                </Grid>
                                <Grid item xs={1}>
                                    <Typography>{Math.round(e.percentage)}%</Typography>
                                </Grid>
                            </Grid>
                        )
                        : <LoadingBox />
                }
            </AccordionDetails>
        </Accordion>
    );
}