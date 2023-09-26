using System;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Models;

namespace Uhost.Core.Services
{
    /// <summary>
    /// Класс для обработки ответа разделенного на странички
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pager<T> : List<T>
    {
        public const int DefCurrentPage = 1;
        public const int DefPerPage = 15;

        public int CurrentPage { get; set; }
        public int PerPage { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)Total / PerPage);
        public int Total { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="data">Данные запроса</param>
        /// <param name="query">Параметры запроса</param>
        /// <param name="exclude">Кол-во искючаемых записей</param>
        public Pager(IEnumerable<T> data, PagedQueryModel query, int exclude = 0)
        {
            CurrentPage = query.Page;
            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }
            PerPage = query.PerPage;
            if (PerPage < 1)
            {
                PerPage = 1;
            }
            if (data == null)
            {
                return;
            }
            if (data is IQueryable<T> q)
            {
                Total = q.Count();
                AddRange(q.Skip((CurrentPage - 1) * PerPage).Take(PerPage - exclude));
            }
            else
            {
                Total = data.Count();
                AddRange(data.Skip((CurrentPage - 1) * PerPage).Take(PerPage - exclude));
            }
        }

        /// <summary>
        /// Возвращаем объект пагинации
        /// </summary>
        /// <returns></returns>
        public object Paginate()
        {
            return new
            {
                Pager = new
                {
                    PerPage,
                    CurrentPage,
                    TotalPages,
                    Total
                },
                Items = this
            };
        }
    }
}
