import { Button, Typography, useMediaQuery } from '@mui/material';
import { useState, useContext } from 'react';
import DeleteIcon from '@mui/icons-material/Delete';
import YesNoDialog from '../YesNoDialog';
import VideoEndpoint from '../../api/VideoEndpoint';
import StateContext from '../../utils/StateContext';
import { red } from '@mui/material/colors';

export default function DeleteVideoDialogButton({ video, setVideo }) {
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
                {!isNarrowScreen && <Typography variant='button'>Удалить</Typography>}
            </Button>
            <YesNoDialog
                visible={visible}
                setVisible={setVisible}
                message={`Вы действительно ходите удалить видео "${video?.name}"?`}
                onYes={onYes}
                onNo={onNo} />
        </div>
    );

}