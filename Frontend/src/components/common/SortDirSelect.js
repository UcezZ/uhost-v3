import { FormControl, InputLabel, MenuItem, Select } from '@mui/material';
import { useTranslation } from 'react-i18next';
import Common from '../../utils/Common';


export default function SortDirSelect({ sortDir, setSortDir }) {
    const { t } = useTranslation();

    return (
        <FormControl>
            <InputLabel id='select-sort-dir'>{t('filter.common.sortdir.caption')}</InputLabel>
            <Select
                labelId='select-sort-dir'
                value={sortDir}
                label={t('filter.common.sortdir.caption')}
                sx={{ minWidth: '180px' }}
                onChange={e => setSortDir && setSortDir(Common.filterSortDirection(e.target.value))}
            >
                {Common.getSortDirections().map((e, i) => <MenuItem key={i} value={e}>{t(`filter.common.sortdir.${e}`)}</MenuItem>)}
            </Select>
        </FormControl>
    );
}