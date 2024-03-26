import { Box, Container } from '@mui/material';
import { useContext } from 'react';
import StateContext from '../../utils/StateContext';
import AddVideoDialogButton from '../video/AddVideoDialogButton';

export default function Videos() {
    const { user } = useContext(StateContext);

    return (
        <Container sx={{ maxWidth: '100% !important' }}>
            {
                user?.id > 0 && <Box sx={{
                    display: 'flex',
                    justifyContent: 'space-around',
                    m: 2
                }}>
                    <AddVideoDialogButton />
                </Box>
            }
        </Container>
    );
}