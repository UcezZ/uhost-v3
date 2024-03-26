import { Drawer, Divider, List, ListItemButton, ListItemIcon, ListItemText, ListItem } from '@mui/material';
import { Link } from 'react-router-dom';
import VideocamIcon from '@mui/icons-material/Videocam';
import config from '../../config.json';
import { useContext } from 'react';
import StateContext from '../../utils/StateContext';
import HomeIcon from '@mui/icons-material/Home';
import PersonIcon from '@mui/icons-material/Person';
import FormatListBulletedIcon from '@mui/icons-material/FormatListBulleted';
import TimelapseIcon from '@mui/icons-material/Timelapse';

const listItemSx =
{
    paddingLeft: 0,
    paddingRight: 0
};
const listItemButtonSx = {
    paddingLeft: 3,
    paddingRight: 3
};

export default function MenuDrawer() {
    const { isDrawerOpen, setIsDrawerOpen } = useContext(StateContext);

    function closeDrawer() {
        setIsDrawerOpen(false);
    }

    return (
        <Drawer
            variant='persistent'
            anchor='left'
            open={isDrawerOpen ?? false}
            sx={{ backgroundColor: '#0008' }}>
            <List sx={{ paddingTop: 8 }}>
                <Link
                    to={`${config.webroot}`}
                    onClick={closeDrawer}>
                    <ListItem sx={{ ...listItemSx }}>
                        <ListItemButton sx={{ ...listItemButtonSx }}>
                            <ListItemIcon>
                                <HomeIcon />
                            </ListItemIcon>
                            <ListItemText primary='Главная' />
                        </ListItemButton>
                    </ListItem>
                </Link>
                <Divider />
                <Link
                    to={`${config.webroot}/profile`}
                    onClick={closeDrawer}>
                    <ListItem sx={{ ...listItemSx }}>
                        <ListItemButton sx={{ ...listItemButtonSx }}>
                            <ListItemIcon>
                                <PersonIcon />
                            </ListItemIcon>
                            <ListItemText primary='Профиль' />
                        </ListItemButton>
                    </ListItem>
                </Link>
                <Link
                    to={`${config.webroot}/videos`}
                    onClick={closeDrawer}>
                    <ListItem sx={{ ...listItemSx }}>
                        <ListItemButton sx={{ ...listItemButtonSx }}>
                            <ListItemIcon>
                                <VideocamIcon />
                            </ListItemIcon>
                            <ListItemText primary='Видео' />
                        </ListItemButton>
                    </ListItem>
                </Link>
                <Link
                    to={`${config.webroot}/video-processing`}
                    onClick={closeDrawer}>
                    <ListItem sx={{ ...listItemSx }}>
                        <ListItemButton sx={{ ...listItemButtonSx }}>
                            <ListItemIcon>
                                <TimelapseIcon />
                            </ListItemIcon>
                            <ListItemText primary='Обработка видео' />
                        </ListItemButton>
                    </ListItem>
                </Link>
                <Link
                    to={`${config.webroot}/playlists`}
                    onClick={closeDrawer}>
                    <ListItem sx={{ ...listItemSx }}>
                        <ListItemButton sx={{ ...listItemButtonSx }}>
                            <ListItemIcon>
                                <FormatListBulletedIcon />
                            </ListItemIcon>
                            <ListItemText primary='Плейлисты' />
                        </ListItemButton>
                    </ListItem>
                </Link>
            </List>
        </Drawer >
    );
}