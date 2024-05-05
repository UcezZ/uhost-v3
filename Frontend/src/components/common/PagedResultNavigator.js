import { Pagination } from "@mui/material";

export default function PagedResultNavigator({ pager, sx, onPageToggle }) {
    if (!pager?.totalPages || pager.totalPages < 2) {
        return;
    }

    return (
        <div style={{ width: '100%', justifyContent: 'center', display: 'flex' }}>
            <Pagination count={pager?.totalPages} page={pager?.currentPage} sx={sx} onChange={(e, p) => onPageToggle && onPageToggle(p)} />
        </div>
    );
}