import { FormControl, InputLabel, MenuItem, Select } from '@mui/material';
import { useTranslation } from 'react-i18next';
import Common from '../../utils/Common';

const PER_PAGE_VALUES = [
    10,
    25,
    50,
    100,
    200,
    300
]

export default function PerPageSelect({ perPage, setPerPage }) {
    const { t } = useTranslation();

    return (
        <FormControl>
            <InputLabel id='select-sort-dir'>{t('filter.common.perpage')}</InputLabel>
            <Select
                labelId='select-sort-dir'
                value={perPage}
                label={t('filter.common.perpage')}
                sx={{ minWidth: '180px' }}
                onChange={e => setPerPage && setPerPage(e.target.value)}
            >
                {PER_PAGE_VALUES.map((e, i) => <MenuItem key={i} value={e}>{e}</MenuItem>)}
            </Select>
        </FormControl>
    );
}