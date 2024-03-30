import { Toolbar, IconButton, AppBar } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import ProfileButton from '../profile/ProfileButton';
import { useContext } from 'react';
import StateContext from '../../utils/StateContext';
import { useLocation, Link } from 'react-router-dom';
import config from '../../config.json';
import NotificationsButton from '../notifications/NotificationsButton';
import CloseIcon from '@mui/icons-material/Close';
import HomeIcon from '@mui/icons-material/Home';

export default function Header() {
    const { user, isDrawerOpen, setIsDrawerOpen } = useContext(StateContext);
    const location = useLocation();
    const isLoginPage = location.pathname?.toLocaleLowerCase() === `${config.webroot}/login`;

    function toggleDrawer() {
        setIsDrawerOpen(!isDrawerOpen);
    }

    return (
        <AppBar
            position='sticky'
            component='nav'
            enableColorOnDark
            sx={{ zIndex: 1250 }}>
            <Toolbar style={{ justifyContent: 'space-between' }}>
                {user
                    ? <IconButton
                        size='large'
                        color='inherit'
                        aria-label='menu'
                        onClick={toggleDrawer}>
                        {isDrawerOpen ? <CloseIcon /> : <MenuIcon />}
                    </IconButton>
                    : <Link to={`${config.webroot}`}>
                        <IconButton>
                            <HomeIcon />
                        </IconButton>
                    </Link>}
                <div style={{ display: 'flex', gap: '1em' }}>
                    {user && <NotificationsButton />}
                    {!isLoginPage && <ProfileButton />}
                </div>
            </Toolbar>
        </AppBar>
    );
}