import { Typography } from '@mui/material';
import { useContext, useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import LogsEndpoint from '../../api/LogsEndpoint';
import StateContext from '../../utils/StateContext';
import LoadingBox from '../LoadingBox';
import LogsComponent from '../logs/LogsComponent';

export default function LogsPage() {
    const [loading, setLoading] = useState(true);
    const [events, setEvents] = useState();
    const { setError } = useContext(StateContext);

    async function onLoad() {
        if (!loading) {
            return;
        }

        await LogsEndpoint.events()
            .then(e => {
                if (e?.data?.success && e?.data?.result) {

                    setEvents(e.data.result);

                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));

        setLoading(false);
    }

    useEffect(() => {
        onLoad();
    }, [loading]);

    if (loading) {
        return (
            <LoadingBox />
        );
    }

    return (
        <LogsComponent events={events} />
    );
}