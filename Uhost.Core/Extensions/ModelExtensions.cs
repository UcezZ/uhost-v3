using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Models;

namespace Uhost.Core.Extensions
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Преобращует сущность <typeparamref name="TEntity"/> в модель <typeparamref name="TModel"/>
        /// </summary>
        /// <typeparam name="TEntity">Сущность</typeparam>
        /// <typeparam name="TModel">Модель</typeparam>
        /// <param name="entity">Сущность</param>
        /// <returns></returns>
        public static TModel ToModel<TEntity, TModel>(this TEntity entity)
            where TEntity : class, new()
            where TModel : IEntityLoadable<TEntity>, new()
        {
            var model = new TModel();
            model.LoadFromEntity(entity);

            return model;
        }

        /// <summary>
        /// Преобразует коллекцию сущностей <typeparamref name="TEntity"/> в коллекцию моделей <typeparamref name="TModel"/>
        /// </summary>
        /// <typeparam name="TEntity">Сущность</typeparam>
        /// <typeparam name="TModel">Модель</typeparam>
        /// <param name="entities">Коллекция сущностей</param>
        /// <returns></returns>
        public static IEnumerable<TModel> ToModelCollection<TEntity, TModel>(this IEnumerable<TEntity> entities)
            where TEntity : class, new()
            where TModel : IEntityLoadable<TEntity>, new()
        {
            return entities.Select(e => e.ToModel<TEntity, TModel>());
        }

        /// <summary>
        /// Преобразует коллекцию сущностей <typeparamref name="TEntity"/> в коллекцию моделей <typeparamref name="TModel"/>
        /// </summary>
        /// <typeparam name="TEntity">Сущность</typeparam>
        /// <typeparam name="TModel">Модель</typeparam>
        /// <param name="entities">Коллекция сущностей</param>
        /// <returns></returns>
        public static IQueryable<TModel> ToModelCollection<TEntity, TModel>(this IQueryable<TEntity> entities)
            where TEntity : class, new()
            where TModel : IEntityLoadable<TEntity>, new()
        {
            return entities.Select(e => e.ToModel<TEntity, TModel>());
        }
    }
}
