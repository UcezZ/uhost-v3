import ImageIcon from '@mui/icons-material/Image';
import { Box } from '@mui/material';
import { useState } from 'react';

export default function SafeImage({ src, alt, height, width, altElement, ...props }) {
    const [success, setSuccess] = useState(true);
    const style = {
        ...props?.sx,
        ...props?.style,
        width: width ?? '100%',
        height: height ?? '100%',
        objectFit: 'cover'
    };

    return success && src?.length > 0
        ? <img
            src={src}
            alt={alt ?? ''}
            onError={e => setSuccess(false)}
            style={style}
            {...props} />
        : altElement ?? <Box
            height={height ?? 'auto'}
            width={width ?? 'auto'}
            {...props}>
            <ImageIcon sx={{
                width: '100%',
                height: '100%'
            }} />
        </Box>
}