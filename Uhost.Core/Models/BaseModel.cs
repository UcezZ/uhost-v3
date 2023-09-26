using System;
using Uhost.Core.Data;

namespace Uhost.Core.Models
{
    /// <summary>
    /// Base model for entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class BaseModel<TEntity> : IEntityLoadable<TEntity> where TEntity : BaseEntity, new()
    {
        public virtual TEntity ToEntity() => FillEntity(new TEntity());

        public virtual TEntity FillEntity(TEntity entity) => throw new NotImplementedException();

        public virtual void LoadFromEntity(TEntity entity) => throw new NotImplementedException();
    }
}
