import ImageIcon from '@mui/icons-material/Image';
import { Box } from '@mui/material';
import { useState } from 'react';

export default function Image({ src, alt, height, width, ...props }) {
    const [success, setSuccess] = useState(true);
    const style = {
        ...props?.sx,
        ...props?.style,
        width: width ?? '100%',
        height: height ?? '100%',
        objectFit: 'cover'
    };

    return success
        ? <img
            src={src}
            alt={alt ?? ''}
            onError={e => setSuccess(false)}
            style={style}
            {...props} />
        : <Box
            fullwidth
            height={height ?? 'auto'}
            width={width ?? 'auto'}
            {...props}>
            <ImageIcon sx={{
                width: '100%',
                height: '100%'
            }} />
        </Box>
}