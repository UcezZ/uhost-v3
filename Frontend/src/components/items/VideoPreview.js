import { Card } from "@mui/material";

export default function VideoPreview({ entity }) {
    return (
        <Card>
            {JSON.stringify(entity, null, 2)}
        </Card>
    )
}