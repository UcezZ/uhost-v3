import { Badge, IconButton } from '@mui/material';
import NotificationsNoneIcon from '@mui/icons-material/NotificationsNone';
import NotificationsIcon from '@mui/icons-material/Notifications';
import { useState } from 'react';

export default function NotificationsButton() {
    const [count, setCount] = useState(0);

    return (
        <IconButton size='large' color='inherit' aria-label='notifications'>
            {
                count > 0
                    ? <Badge badgeContent={count} color='secondary'>
                        <NotificationsIcon />
                    </Badge>
                    : <NotificationsNoneIcon />
            }
        </IconButton>
    );
}