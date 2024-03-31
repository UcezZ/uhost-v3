import { Button, Typography, useMediaQuery } from '@mui/material';
import { useState, useContext } from 'react';
import DeleteIcon from '@mui/icons-material/Delete';
import YesNoDialog from '../YesNoDialog';
import VideoEndpoint from '../../api/VideoEndpoint';
import StateContext from '../../utils/StateContext';
import { useTranslation } from 'react-i18next';

export default function DeleteVideoDialogButton({ video, setVideo }) {
    const { t } = useTranslation();
    const { setError } = useContext(StateContext);
    const [visible, setVisible] = useState(false);
    const isNarrowScreen = useMediaQuery('(max-width:600px)');

    async function onYes() {
        await VideoEndpoint.delete(video.token)
            .then(e => {
                if (e?.data?.success && e?.data?.result) {
                    setVideo();
                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));

        setVisible(false);
    };

    function onNo() {
        setVisible(false);
    };

    return (
        <div>
            <Button
                size='large'
                variant='contained'
                onClick={e => setVisible(true)}
                color='primary'
                sx={{ gap: 1 }}>
                <DeleteIcon />
                {!isNarrowScreen && <Typography variant='button'>{t('video.delete')}</Typography>}
            </Button>
            <YesNoDialog
                visible={visible}
                setVisible={setVisible}
                message={t('video.delete.confirm', { name: video?.name ?? 'N/A' })}
                onYes={onYes}
                onNo={onNo} />
        </div>
    );

}