using System;
using System.Linq;
using Uhost.Core.Data;

namespace Uhost.Core.Models
{
    /// <summary>
    /// Base model for entity collection
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class BaseCollectionModel<TEntity> : IEntityCollectionLoadable<TEntity> where TEntity : BaseEntity, new()
    {
        public virtual void LoadFromEntityCollection(IQueryable<TEntity> entities) => throw new NotImplementedException();
    }
}
