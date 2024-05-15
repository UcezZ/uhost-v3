import { Box, Button } from '@mui/material';
import { useContext, useEffect, useState } from 'react';
import Common from '../../utils/Common';
import ThumbUpIcon from '@mui/icons-material/ThumbUp';
import ThumbDownIcon from '@mui/icons-material/ThumbDown';
import ReactionEndpoint from '../../api/ReactionEndpoint';
import StateContext from '../../utils/StateContext';

/**
 * 
 * @param {Number} value 
 * @returns {String}
 */
function reactionCountToText(value) {
    if (value <= 0) {
        return '0';
    }

    if (value < 1000) {
        return value.toString();
    }

    if (value < 10000) {
        return `${Math.round(value / 100.0) / 10.0} K`;
    }

    return `${Math.round(value / 1000.0)} K`;
}

const REACTIONS = [
    {
        key: 'dislike',
        icon: <ThumbDownIcon />
    },
    {
        key: 'like',
        icon: <ThumbUpIcon />
    }
];

export default function ReactionSection({ token }) {
    const [loading, setLoading] = useState(true);
    const [postLoading, setPostLoading] = useState(false);
    const [reactions, setReactions] = useState({});
    const [currentReaction, setCurrentReaction] = useState();
    const { setError, user } = useContext(StateContext);

    function handleReactionStats(response) {
        if (response?.data?.success && response?.data?.result?.reactions) {
            setReactions(response.data.result.reactions);
            setCurrentReaction(response.data.result.currentUserReaction);
        } else {
            setError(Common.transformErrorData(response));
        }
    }

    async function onLoad() {
        if (!loading) {
            return;
        }

        await ReactionEndpoint.get(token)
            .then(handleReactionStats)
            .catch(e => setError(Common.transformErrorData(e)));
        setLoading(false);
    }

    /**
     * 
     * @param {String} value 
     */
    async function onReactionPost(value) {
        await ReactionEndpoint.post(token, value)
            .then(handleReactionStats)
            .catch(e => setError(Common.transformErrorData(e)));
    }

    async function onReactionDelete() {
        await ReactionEndpoint.delete(token)
            .then(handleReactionStats)
            .catch(e => setError(Common.transformErrorData(e)));
    }

    async function onReactionChanged(value) {
        setPostLoading(true);
        if (value === currentReaction) {
            await onReactionDelete();
        } else {
            await onReactionPost(value);
        }
        setPostLoading(false);
    }

    useEffect(() => {
        onLoad();
    }, [loading]);

    useEffect(() => setLoading(true), [user]);

    function oneReactionButton(data, index) {
        const equalsCurrent = data.key === currentReaction;
        const textValue = data.key in reactions
            ? reactionCountToText(reactions[data.key])
            : 0;

        return (
            <Button
                key={index}
                size='large'
                startIcon={data.icon}
                disabled={!user?.id || loading || postLoading}
                onClick={ev => onReactionChanged(data.key)}
                variant={equalsCurrent ? 'contained' : 'outlined'}
                color={equalsCurrent ? 'primary' : 'inherit'}
                sx={{
                    borderTopLeftRadius: index > 0 ? 0 : '0.25em',
                    borderBottomLeftRadius: index > 0 ? 0 : '0.25em',
                    borderTopRightRadius: index < REACTIONS.length - 1 ? 0 : '0.25em',
                    borderBottomRightRadius: index < REACTIONS.length - 1 ? 0 : '0.25em'
                }}
            >
                {textValue}
            </Button>
        );
    }

    return (
        <div style={{
            borderRadius: '0.25em',
            overflow: 'hidden',
            borderColor: 'primary',
            borderWidth: '1px',
            display: 'flex',
            flexWrap: 'nowrap'
        }}>
            {REACTIONS.map(oneReactionButton)}
        </div>
    );
}