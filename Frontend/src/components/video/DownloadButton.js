import { Button, Typography, useMediaQuery } from '@mui/material';
import FileDownloadIcon from '@mui/icons-material/FileDownload';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import { useState } from 'react';
import config from '../../config.json';
import Common from '../../utils/Common';

export default function DownloadButton({ token, sizes }) {
    if (!token || !sizes?.length || !sizes?.map) {
        return;
    }

    const [anchorEl, setAnchorEl] = useState(null);
    const isNarrowScreen = useMediaQuery('(max-width:600px)');

    function onClick(event) {
        setAnchorEl(event.currentTarget);
    };
    function onClose() {
        setAnchorEl(null);
    };
    function onDownloadClick(link) {
        Common.openDownloadUrl(`${config.apiroot}/videos/${token}/download/${link.key.toPascalCase()}`);
        onClose();
    }

    return (
        <div>
            <Button
                size='large'
                color='inherit'
                aria-label='profile'
                id='download-button'
                aria-controls={anchorEl ? undefined : 'download-menu'}
                aria-haspopup='true'
                aria-expanded={anchorEl ? undefined : 'true'}
                variant='contained'
                onClick={onClick}
                sx={{ gap: 1 }}>
                <FileDownloadIcon />
                {!isNarrowScreen && <Typography variant='button'>Скачать</Typography>}
            </Button>
            <Menu
                id='download-menu'
                anchorEl={anchorEl}
                open={!!anchorEl}
                onClose={onClose}
                MenuListProps={{
                    'aria-labelledby': 'download-button',
                }}
            >
                {
                    sizes.map((e, i) =>
                        <MenuItem onClick={ev => onDownloadClick(e)} key={i}>
                            <Typography variant='body1'>{e.name}</Typography>
                            <Typography sx={{ fontStyle: 'italic', marginLeft: 1 }} variant='body2'>({e.size})</Typography>
                        </MenuItem>)
                }
            </Menu>
        </div>
    );
}