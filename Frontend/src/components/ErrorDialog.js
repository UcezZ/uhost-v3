import * as React from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Slide from '@mui/material/Slide';
import StateContext from './../context/StateContext';

const Transition = React.forwardRef(function Transition(props, ref) {
  return <Slide direction="up" ref={ref} {...props} />;
});

var errContent;

export default function ErrorDialog() {
  const { error, setError } = React.useContext(StateContext);

  const handleClose = () => {
    setError(null);
  };

  if (error) {
    errContent = error;
  }

  return (
    <div>
      <Dialog
        open={error != null}
        TransitionComponent={Transition}
        keepMounted
        onClose={handleClose}
        aria-describedby="alert-dialog-slide-description"
      >
        <DialogTitle>Ошибка</DialogTitle>
        <DialogContent color='red'>
          {/* <DialogContentText id="alert-dialog-slide-description"> */}
          <div>{errContent}</div>
          {/* </DialogContentText> */}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose}>Закрыть</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}