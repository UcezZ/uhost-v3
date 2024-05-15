import { Accordion, AccordionDetails, AccordionSummary } from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { useTranslation } from 'react-i18next';
import CodeBlock from '../common/CodeBlock';

export default function LogItem({ item }) {
    const { t } = useTranslation();
    const hasData = /^\s*\{\s*\S+\s*\}\s*$/gm.test(item?.data);

    return (
        <Accordion sx={{ mt: 1, mb: 1 }}>
            <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                #{item?.id} | {item?.createdAt} | {t(`logs.events.${item?.event?.toString().toKebabCase() ?? 'undefined'}`)}
            </AccordionSummary>
            <AccordionDetails>
                <div>
                    <p><b>{t('logs.details.createdAt')}:</b> {`${item?.createdAtDetail}`}</p>
                    {item?.userId > 0 && <p><b>{t('logs.details.user')}:</b> {`#${item?.userId} | ${item?.user?.login} | ${item?.user?.lastVisitAt}`}</p>}
                    {item?.ipAddress?.length > 0 && <p><b>{t('logs.details.ip')}:</b> {`${item?.ipAddress}`}</p>}
                    {hasData && <p><b>{t('logs.details.data')}:</b></p>}
                </div>
                {hasData && <CodeBlock data={item?.data} json />}
            </AccordionDetails>
        </Accordion>
    );
}