import { forwardRef } from 'react';
import Slide from '@mui/material/Slide';

const PopupTransition = forwardRef(function Transition(props, ref) {
    return <Slide direction='up' ref={ref} {...props} />;
});

export default PopupTransition;