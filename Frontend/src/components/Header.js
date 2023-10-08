import { Toolbar, IconButton, AppBar } from "@mui/material";
import MenuIcon from '@mui/icons-material/Menu';
import NotificationsNoneIcon from '@mui/icons-material/NotificationsNone';
import ProfileButton from "./ProfileButton";

export default function Header() {
    return (
        <AppBar position="sticky" component="nav">
            <Toolbar style={{ justifyContent: 'space-between' }}>
                <IconButton size="large" color="inherit" aria-label="menu">
                    <MenuIcon />
                </IconButton>
                <div style={{ display: 'flex', gap: '1em' }}>
                    <IconButton size="large" color="inherit" aria-label="notifications">
                        <NotificationsNoneIcon />
                    </IconButton>
                    <ProfileButton />
                </div>
            </Toolbar>
        </AppBar>
    );
}