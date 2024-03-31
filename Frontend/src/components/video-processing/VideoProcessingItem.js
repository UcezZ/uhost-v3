import { Accordion, AccordionDetails, AccordionSummary, Box, Grid, LinearProgress, Typography } from '@mui/material';
import Image from '../Image';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { useState, useEffect } from 'react';
import { green, grey, red, yellow } from '@mui/material/colors';
import Common from '../../utils/Common';
import VideoEndpoint from '../../api/VideoEndpoint';
import Styles from '../../ui/Styles';
import LoadingBox from '../LoadingBox';

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
            key: e,
            state: data.states[e],
            percentage: percentageWithState(e in data?.progresses ? data.progresses[e] : 0, data.states[e]),
            isWaiting: data.states[e] === STATE_PENDING
        };
    });
}

export default function VideoProcessingItem({ video, expanded }) {
    const [isExpanded, setIsExpanded] = useState(Boolean(expanded) ?? false);
    const [data, setData] = useState([]);
    const [lastUpdate, setLastUpdate] = useState(new Date().getTime());
    const [allCompleded, setAllCompleded] = useState(false);

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
                mb: 1
            }} >
            <AccordionSummary expandIcon={<ExpandMoreIcon />} >
                <Grid container>
                    <Grid item>
                        <Image
                            src={video?.thumbnailUrl}
                            height={90}
                            width={160} />
                    </Grid>
                    <Grid
                        item
                        sx={{
                            display: 'flex',
                            gap: 1,
                            justifyContent: 'center',
                            flexDirection: 'column',
                            pl: 2,
                            pb: 3,
                            pt: 1
                        }} >
                        <Typography
                            variant='h6'
                            component='div'
                            sx={{
                                fontSize: 16,
                                fontWeight: 700
                            }}
                            noWrap>
                            {video?.state ?? 'N/A'}
                        </Typography>
                        <Typography
                            component='div'
                            noWrap>
                            {video?.name ?? 'N/A'}
                        </Typography>
                    </Grid>
                </Grid>
            </AccordionSummary>
            <AccordionDetails>
                {
                    data?.length > 0
                        ? data.map((e, i) =>
                            <Box key={i}>
                                <Typography>{e.key}</Typography>
                                <Typography>{e.state}</Typography>
                                <LinearProgress
                                    value={e.percentage}
                                    sx={{
                                        flex: 1,
                                        height: 16,
                                        borderRadius: 16
                                    }}
                                    variant={e.isWaiting ? 'indeterminate' : 'determinate'} />
                                <Typography>{e.percentage}</Typography>
                            </Box>
                        )
                        //<pre style={Styles.code}>{JSON.stringify(data, null, 2)}</pre>
                        : <LoadingBox />
                }
            </AccordionDetails>
        </Accordion>
    );
}