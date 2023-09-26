using System.Linq;
using Uhost.Core.Models;
using Uhost.Core.Services;

namespace Uhost.Core.Extensions
{
    public static class PagerExtensions
    {
        /// <summary>
        /// Создаёт объект <see cref="Pager{TModel}"/>
        /// </summary>
        /// <typeparam name="TModel">Тип целевой модели сущности</typeparam>
        /// <typeparam name="TQueryModel">Тип модели запроса с пагинацией</typeparam>
        /// <param name="modelsQueryable">Выходной <see cref="IQueryable{TModel}"/> из <c>_repo.GetAll</c></param>
        /// <param name="query">Модель запроса</param>
        /// <returns></returns>
        public static Pager<TModel> CreatePager<TModel, TQueryModel>(this IQueryable<TModel> modelsQueryable, TQueryModel query)
            where TModel : class
            where TQueryModel : PagedQueryModel
        {
            return new Pager<TModel>(modelsQueryable, query);
        }
    }
}
