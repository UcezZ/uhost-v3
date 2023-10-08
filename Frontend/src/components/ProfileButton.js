import { IconButton } from '@mui/material';
import AccountCircleIcon from '@mui/icons-material/AccountCircle';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import { useState } from "react";
import YesNoDialog from './YesNoDialog';
import AuthEndpoint from '../api/AuthEndpoint';

export default function ProfileButton() {
    const [anchorEl, setAnchorEl] = useState(null);
    const [confirmLogout, setConfirmLogout] = useState(null);
    const open = Boolean(anchorEl);
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
    }

    return (
        <div>
            <IconButton
                size="large"
                color="inherit"
                aria-label="profile"
                id="profile-button"
                aria-controls={open ? 'profile-menu' : undefined}
                aria-haspopup="true"
                aria-expanded={open ? 'true' : undefined}
                onClick={onClick}>
                <AccountCircleIcon />
            </IconButton>
            <Menu
                id="profile-menu"
                anchorEl={anchorEl}
                open={open}
                onClose={onClose}
                MenuListProps={{
                    'aria-labelledby': 'profile-button',
                }}
            >
                <MenuItem onClick={onClose}>Profile</MenuItem>
                <MenuItem onClick={onLogoutClick}>Logout</MenuItem>
            </Menu>
            <YesNoDialog
                onNo={onLogoutNo}
                onYes={onLogoutYes}
                visible={confirmLogout}
                setVisible={setConfirmLogout}
                message='Вы действительно хотите выйти?'
            />
        </div>
    );
}