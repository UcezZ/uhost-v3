﻿namespace Uhost.Core.Models
{
    public class PagerInfoModel
    {
        public int PerPage { get; init; }
        public int CurrentPage { get; init; }
        public int TotalPages { get; init; }
        public int Total { get; init; }
    }
}
