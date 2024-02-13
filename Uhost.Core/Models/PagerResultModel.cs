using System.Collections.Generic;
using Uhost.Core.Services;

namespace Uhost.Core.Models
{
    public class PagerResultModel<T>
    {
        public PagerInfoModel Pager { get; private init; }
        public IReadOnlyCollection<T> Items { get; private init; }

        private PagerResultModel() { }

        public static implicit operator PagerResultModel<T>(Pager<T> pager)
        {
            return new PagerResultModel<T>
            {
                Pager = new PagerInfoModel
                {
                    PerPage = pager.PerPage,
                    CurrentPage = pager.CurrentPage,
                    TotalPages = pager.TotalPages,
                    Total = pager.Total,
                    HasNextPage = pager.HasNextPage,
                    HasPreviousPage = pager.HasPreviousPage,
                },
                Items = pager
            };
        }
    }
}
