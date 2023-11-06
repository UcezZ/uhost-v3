using System.Linq;

namespace Uhost.Core.Models
{
    /// <summary>
    /// Поддерживает загрузку данных из сущности
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IEntityCollectionLoadable<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// Загрузка данных из коллекции сущностей <typeparamref name="TEntity"/>
        /// </summary>
        /// <param name="entities">Сущности</param>
        void LoadFromEntityCollection(IQueryable<TEntity> entities);
    }
}
