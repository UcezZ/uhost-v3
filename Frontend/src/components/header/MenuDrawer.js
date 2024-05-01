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
import { t } from 'i18next';
import Rights from '../../utils/Rights';
import NotesIcon from '@mui/icons-material/Notes';
import VpnKeyIcon from '@mui/icons-material/VpnKey';

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
    const { isDrawerOpen, setIsDrawerOpen, user } = useContext(StateContext);

    function closeDrawer() {
        setIsDrawerOpen(false);
    }

    const hasAnyAdminRight = Rights.checkAnyRight(user, Rights.AdminLogAccess, Rights.AdminSessionAccess, Rights.AdminSessionTerminate);

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
                            <ListItemText primary={t('menu.home')} />
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
                            <ListItemText primary={t('menu.profile')} />
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
                            <ListItemText primary={t('menu.video')} />
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
                            <ListItemText primary={t('menu.videoprocessing')} />
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
                            <ListItemText primary={t('menu.playlists')} />
                        </ListItemButton>
                    </ListItem>
                </Link>
                {hasAnyAdminRight && <Divider />}
                {
                    Rights.checkAnyRight(user, Rights.AdminLogAccess) && <Link
                        to={`${config.webroot}/admin/logs`}
                        onClick={closeDrawer}>
                        <ListItem sx={{ ...listItemSx }}>
                            <ListItemButton sx={{ ...listItemButtonSx }}>
                                <ListItemIcon>
                                    <NotesIcon />
                                </ListItemIcon>
                                <ListItemText primary={t('menu.logs')} />
                            </ListItemButton>
                        </ListItem>
                    </Link>
                }
                {
                    Rights.checkAnyRight(user, Rights.AdminSessionAccess, Rights.AdminSessionTerminate) && <Link
                        to={`${config.webroot}/admin/sessions`}
                        onClick={closeDrawer}>
                        <ListItem sx={{ ...listItemSx }}>
                            <ListItemButton sx={{ ...listItemButtonSx }}>
                                <ListItemIcon>
                                    <VpnKeyIcon />
                                </ListItemIcon>
                                <ListItemText primary={t('menu.sessions')} />
                            </ListItemButton>
                        </ListItem>
                    </Link>
                }
            </List>
        </Drawer >
    );
}