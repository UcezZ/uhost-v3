import { IconButton } from '@mui/material';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import { useContext, useState } from 'react';
import YesNoDialog from '../YesNoDialog';
import AuthEndpoint from '../../api/AuthEndpoint';
import { Link } from 'react-router-dom';
import config from '../../config.json';
import StateContext from '../../utils/StateContext';
import PopupAuthForm from '../forms/PopupAuthForm';

export default function ProfileButton() {
    const [anchorEl, setAnchorEl] = useState(null);
    const [confirmLogout, setConfirmLogout] = useState(null);
    const [authFormVisible, setAuthFormVisible] = useState(false);
    const { user, setUser } = useContext(StateContext);

    function onClick(event) {
        setAnchorEl(event.currentTarget);
    };
    function onClose() {
        setAnchorEl(null);
    };
    function onLogoutClick(event) {
        onClose();
        setConfirmLogout(true);
    }
    function onLogoutNo(event) {
        setConfirmLogout(false);
    }
    async function onLogoutYes(event) {
        await AuthEndpoint.logout()
            .then(e => {
                localStorage.removeItem('accessToken');
                sessionStorage.removeItem('accessToken');
            })
            .catch(e => { });

        setConfirmLogout(false);
        setUser();
    }

    function onLoginClick(event) {
        setAuthFormVisible(true);
        onClose();
    }

    return (
        <div>
            <IconButton
                size="large"
                color="inherit"
                aria-label="profile"
                id="profile-button"
                aria-controls={anchorEl ? undefined : 'profile-menu'}
                aria-haspopup="true"
                aria-expanded={anchorEl ? undefined : 'true'}
                onClick={onClick}>
                <AccountCircleIcon />
            </IconButton>
            <Menu
                id="profile-menu"
                anchorEl={anchorEl}
                open={!!anchorEl}
                onClose={onClose}
                MenuListProps={{
                    'aria-labelledby': 'profile-button',
                }}
            >
                {!user && <MenuItem onClick={onLoginClick}>Log in</MenuItem>}
                {user && <MenuItem onClick={onClose}><Link to={`${config.webroot}/profile`}>Profile</Link></MenuItem>}
                {user && <MenuItem onClick={onLogoutClick}>Log out</MenuItem>}
            </Menu>
            <YesNoDialog
                onNo={onLogoutNo}
                onYes={onLogoutYes}
                visible={confirmLogout}
                setVisible={setConfirmLogout}
                message='Вы действительно хотите выйти?'
            />
            <PopupAuthForm visible={authFormVisible} setVisible={setAuthFormVisible} next={onClose} />
        </div>
    );
}