import { Accordion, AccordionDetails, AccordionSummary } from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { useTranslation } from 'react-i18next';
import CodeBlock from '../common/CodeBlock';

export default function LogItem({ item }) {
    const { t } = useTranslation();

    return (
        <Accordion sx={{ mt: 1, mb: 1 }}>
            <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                #{item?.id} | {item?.createdAt} | {t(`logs.events.${item?.event?.toString().toKebabCase() ?? 'undefined'}`)}
            </AccordionSummary>
            <AccordionDetails>
                <div>
                    <p><b>{t('logs.details.createdAt')}:</b> {`${item?.createdAtDetail}`}</p>
                    <p><b>{t('logs.details.user')}:</b> {`#${item?.userId} | ${item?.user?.login} | ${item?.user?.lastVisitAt}`}</p>
                    <p><b>{t('logs.details.ip')}:</b> {`${item?.ipAddress}`}</p>
                    <p><b>{t('logs.details.data')}:</b></p>
                </div>
                <CodeBlock data={item?.data} json />
            </AccordionDetails>
        </Accordion>
    );
}