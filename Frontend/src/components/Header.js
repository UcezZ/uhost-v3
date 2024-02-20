import { Toolbar, IconButton, AppBar } from "@mui/material";
import MenuIcon from '@mui/icons-material/Menu';
import NotificationsNoneIcon from '@mui/icons-material/NotificationsNone';
import ProfileButton from "./items/ProfileButton";
import { useContext } from "react";
import StateContext from "../utils/StateContext";
import { useLocation } from "react-router-dom";
import config from '../config.json';

export default function Header() {
    const { user } = useContext(StateContext);
    const location = useLocation();
    const isLoginPage = location.pathname?.toLocaleLowerCase() === `${config.webroot}/login`;

    return (
        <AppBar position="sticky" component="nav" >
            <Toolbar style={{ justifyContent: 'space-between' }}>
                <IconButton size="large" color="inherit" aria-label="menu">
                    <MenuIcon />
                </IconButton>
                <div style={{ display: 'flex', gap: '1em' }}>
                    {user && <IconButton size="large" color="inherit" aria-label="notifications"><NotificationsNoneIcon /></IconButton>}
                    {!isLoginPage && <ProfileButton />}
                </div>
            </Toolbar>
        </AppBar>
    );
}