using Uhost.Core.Data;

namespace Uhost.Core.Models
{
    /// <summary>
    /// Поддерживает загрузку данных из сущности
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IEntityLoadable<TEntity> where TEntity :BaseEntity, new()
    {
        /// <summary>
        /// Загрузка данных из сущности <typeparamref name="TEntity"/>
        /// </summary>
        /// <param name="entity">Сущность</param>
        void LoadFromEntity(TEntity entity);
    }
}
