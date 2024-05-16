import { useContext, useEffect, useState } from 'react';
import LogsEndpoint from '../../api/LogsEndpoint';
import Common from '../../utils/Common';
import StateContext from '../../utils/StateContext';
import LoadingBox from '../common/LoadingBox';
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