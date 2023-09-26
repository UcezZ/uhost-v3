namespace Uhost.Core.Models
{
    public interface IEntityLoadable<TEntity> where TEntity : class, new()
    {
        void LoadFromEntity(TEntity entity);

        TEntity FillEntity(TEntity entity);
    }
}
