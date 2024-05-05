import React, { useContext, useState, useEffect } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import FormControlLabel from '@mui/material/FormControlLabel';
import Checkbox from '@mui/material/Checkbox';
import Box from '@mui/material/Box';
import StateContext from '../../utils/StateContext';
import { CircularProgress, FormControl, FormGroup } from '@mui/material';
import Common from '../../utils/Common';
import RoleEndpoint from '../../api/RoleEndpoint';
import Validation from '../../utils/Validation';
import Styles from '../../ui/Styles';
import { useTranslation } from 'react-i18next';

export default function EditRoleForm({ role, allRights, next, onClose }) {
    const { t } = useTranslation();
    const { setError } = useContext(StateContext);
    const [loading, setLoading] = useState(false);
    const [name, setName] = useState(role?.name ?? '');
    const [rights, setRights] = useState(role?.rights ?? []);

    async function onSubmit() {
        setLoading(true);
        await RoleEndpoint.update(role.id, name, rights)
            .then(e => {
                if (e?.data?.success && e?.data?.result) {
                    setName('');
                    setRights('');

                    next && next();
                } else {
                    setError(Common.transformErrorData(e));
                }
            })
            .catch(e => setError(Common.transformErrorData(e)));
        setLoading(false);
    }

    function isValid() {
        return Validation.Role.name(name);
    }

    function onRightCheck(value, checked) {
        if (checked) {
            setRights([...rights, value]);
        } else {
            setRights(rights.filter(e => e != value));
        }
    }

    console.log(allRights.map(e => `"right.${e.toString().toKebabCase()}":""`).join(','))

    return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
            }}
        >
            <Box noValidate sx={{ mt: 1 }}>
                <TextField
                    margin='normal'
                    required
                    fullWidth
                    label={t('role.name')}
                    error={!Validation.Role.name(name)}
                    disabled={loading}
                    value={name}
                    onChange={e => setName(e.target.value)}
                    autoFocus
                />
                <FormControl component='fieldset'
                    sx={{
                        overflowY: 'auto',
                        scrollbarWidth: 'thin',
                        maxHeight: '300px',
                        width: '100%'
                    }}>
                    <FormGroup>
                        {
                            allRights.map((e, i) => <FormControlLabel
                                key={i}
                                control={<Checkbox color='primary' checked={rights.includes(e)} onChange={(_, c) => onRightCheck(e, c)} />}
                                label={t(`right.${e.toString().toKebabCase()}`)}
                                sx={{ ...Styles.noSelectSx, ml: 1 }}
                            />)
                        }
                    </FormGroup>
                </FormControl>
                <Box sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    alignItems: 'center',
                    gap: 2,
                    ...Styles.noSelectSx
                }}>
                    <Button
                        fullWidth
                        variant='outlined'
                        disabled={loading}
                        sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                        onClick={onClose}
                    >
                        {t('common.cancel')}
                    </Button>
                    <Button
                        type='submit'
                        fullWidth
                        variant='contained'
                        disabled={loading || !isValid()}
                        sx={{ mt: 3, mb: 2, p: 1, minHeight: '40px' }}
                        onClick={onSubmit}
                    >
                        {loading ? <CircularProgress size={20} /> : t('common.apply')}
                    </Button>
                </Box>
            </Box>
        </Box>
    );
}