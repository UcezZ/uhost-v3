using Uhost.Core.Data;

namespace Uhost.Core.Models
{
    /// <summary>
    /// Поддерживает запись данных в сущность
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IEntityFillable<TEntity> where TEntity : BaseEntity, new()
    {
        /// <summary>
        /// Заполнение данными сущности <typeparamref name="TEntity"/>
        /// </summary>
        /// <param name="entity">Сущность</param>
        TEntity FillEntity(TEntity entity);
    }
}