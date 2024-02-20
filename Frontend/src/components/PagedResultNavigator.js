import { Pagination } from "@mui/material";

export default function PagedResultNavigator({ pager, sx, onPageToggle }) {
    return (
        <div style={{ width: '100%', justifyContent: 'center', display: 'flex' }}>
            <Pagination count={pager?.totalPages} page={pager?.currentPage} sx={sx} onChange={(e, p) => onPageToggle && onPageToggle(p)} />
        </div>
    );
}