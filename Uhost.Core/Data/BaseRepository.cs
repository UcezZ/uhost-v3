using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models;

namespace Uhost.Core.Common
{
    public abstract class BaseRepository<TEntity> : IDisposable, IAsyncDisposable
        where TEntity : BaseEntity, new()
    {
        private DbSet<TEntity> _objectSet;
        protected readonly DbContext _dbContext;

        public static string TableName => Tools.GetEntityTableName<TEntity>();

        protected virtual Func<IQueryable<TEntity>, IQueryable<TEntity>> DbSetUpdateTransformations => e => e;

        protected DbSet<TEntity> DbSet => _objectSet ??= _dbContext.Set<TEntity>();

        public BaseRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Получение всех <typeparamref name="TEntity"/>
        /// Нужно быть внимательным т.к связи не загружаются - использовать EntityFramework, в не System.Data.Entity когда в репо задействуются .Include() и .ThenInclude()
        /// </summary>
        public virtual IQueryable<TModel> Get<TModel>(IQueryable<TEntity> query = null) where TModel : IEntityLoadable<TEntity>, new()
        {
            query ??= DbSet.AsNoTracking();

            return query.ToModelCollection<TEntity, TModel>();
        }

        /// <summary>
        /// Получение всех <typeparamref name="TEntity"/>
        /// Нужно быть внимательным т.к связи не загружаются - использовать EntityFramework, в не System.Data.Entity когда в репо задействуются .Include() и .ThenInclude()
        /// </summary>
        public virtual TCollectionModel GetCollection<TCollectionModel>(IQueryable<TEntity> query = null) where TCollectionModel : IEntityCollectionLoadable<TEntity>, new()
        {
            query ??= DbSet.AsNoTracking();
            var model = new TCollectionModel();
            model.LoadFromEntityCollection(query);

            return model;
        }

        /// <summary>
        /// Получает все сущности <typeparamref name="TEntity"/> по SQL запросу
        /// </summary>
        /// <param name="sql">SQL запрос</param>
        /// <param name="predicate">Дополнительные условия, например Include</param>
        /// <returns></returns>
        public IQueryable<TEntity> GetEntitiesFromSql(string sql, Func<IQueryable<TEntity>, IQueryable<TEntity>> predicate = null)
        {
            IQueryable<TEntity> q = DbSet.FromSqlRaw(sql);

            if (predicate != null)
            {
                q = predicate.Invoke(q);
            }

            return q;
        }

        /// <summary>
        /// Получение всех моделей <typeparamref name="TModel"/> по SQL запросу
        /// </summary>
        /// <typeparam name="TModel">Целевая модель</typeparam>
        /// <param name="sql">SQL запрос</param>
        /// <param name="predicate">Дополнительные условия, например Include</param>
        /// <returns></returns>
        public IQueryable<TModel> GetFromSql<TModel>(string sql, Func<IQueryable<TEntity>, IQueryable<TEntity>> predicate = null)
            where TModel : IEntityLoadable<TEntity>, new()
        {
            return GetEntitiesFromSql(sql, predicate).ToModelCollection<TEntity, TModel>();
        }

        /// <summary>
        /// Проверка существования в базе наборка Entites
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public bool Exists(IQueryable<TEntity> entities)
        {
            return entities.Any();
        }

        /// <summary>
        /// Получение <typeparamref name="TEntity"/> по Id
        /// Нужно быть внимательным т.к связи не загружаются 
        /// </summary>
        public bool FindEntity(Expression<Func<TEntity, bool>> selector, out TEntity item)
        {
            item = DbSetUpdateTransformations.Invoke(DbSet).FirstOrDefault(selector);

            return item != null;
        }

        /// <summary>
        /// Получение <typeparamref name="TEntity"/> по Id
        /// Нужно быть внимательным т.к связи не загружаются 
        /// </summary>
        public bool FindEntity(int id, out TEntity item)
        {
            return FindEntity(e => e.Id == id, out item);
        }

        /// <summary>
        /// Получение коллекции <typeparamref name="TEntity"/> по условию
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public bool FindEntities(Expression<Func<TEntity, bool>> selector, out IEnumerable<TEntity> entities)
        {
            entities = DbSetUpdateTransformations.Invoke(DbSet).Where(selector).ToList();

            return entities != null && entities.Any();
        }

        /// <summary>
        /// Получение коллекции <typeparamref name="TEntity"/> по коллекции Id
        /// Нужно быть внимательным т.к связи не загружаются 
        /// </summary>
        public bool FindEntities(IEnumerable<int> ids, out IEnumerable<TEntity> entities)
        {
            return FindEntities(e => ids.Contains(e.Id), out entities);
        }

        /// <summary>
        /// Поиск одной <typeparamref name="TEntity"/> и возвращение в качестве <typeparamref name="TModel"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool FindOneViewModel<TModel>(int id, out TModel model) where TModel : IEntityLoadable<TEntity>, new()
        {
            if (FindEntity(id, out TEntity entity))
            {
                model = entity.ToModel<TEntity, TModel>();
                return true;
            }

            model = default;
            return false;
        }

        /// <summary>
        /// Сохранение
        /// </summary>
        public int Save()
        {
            try
            {
                return _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                }

                throw e; // исключительно для отладки
                throw;
            }
            // dbContext.DetachAllEntities();
        }

        /// <summary>
        /// Сохранение
        /// </summary>
        public async Task<int> SaveAsync()
        {
            try
            {
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                }

                throw e; // исключительно для отладки
                throw;
            }
            // dbContext.DetachAllEntities();
        }

        /// <summary>
        /// После создания <typeparamref name="TEntity"/> часто появляется необходимость перезагрузить данные <typeparamref name="TEntity"/> и все её связи
        /// Это можно сделать при помощи данного метода
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpression"></param>
        public void SaveAndReload(TEntity entity, Expression<Func<TEntity, object>> propertyExpression = null)
        {
            Save();

            _dbContext.Entry(entity).Reload();

            if (propertyExpression != null)
            {
                _dbContext.Entry(entity).Reference(propertyExpression).Load();
            }
        }

        /// <summary>
        /// Установка значение поля типа DateTime
        /// </summary>
        protected void SetEntityDateTimeField(TEntity entity, string fieldName, DateTime value)
        {
            entity?.GetType()
                .GetProperty(fieldName)?
                .SetValue(entity, value);
        }

        /// <summary>
        /// Мягкое удаление <typeparamref name="TEntity"/>
        /// Установка поля deletedAt в CURRENT_TIMESTAMP
        /// </summary>
        /// <param name="id">ИД сущности</param>
        /// <param name="save">Сохранить изменения. По умолчанию true, установить в false в составе транзакции</param>
        public virtual void SoftDelete(int id, bool save = true)
        {
            if (typeof(TEntity).IsAssignableTo<BaseDateTimedEntity>() && FindEntity(id, out var entity) && entity is BaseDateTimedEntity dtEntity)
            {
                dtEntity.DeletedAt = DateTime.Now;

                if (save)
                {
                    Save();
                }
            }
        }

        /// <summary>
        /// Создание <typeparamref name="TEntity"/> из <typeparamref name="TModel"/>
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="setTimeField">Установка CreatedAt</param>
        /// <returns></returns>
        public TEntity Add<TModel>(TModel model, bool setTimeField = true) where TModel : IEntityFillable<TEntity>
        {
            var entity = model.ToEntity();

            if (setTimeField && entity is BaseDateTimedEntity dtEntity)
            {
                dtEntity.CreatedAt = DateTime.Now;
            }

            DbSet.Add(entity);
            Save();

            return entity;
        }

        /// <summary>
        /// Асинхронное создание <typeparamref name="TEntity"/> из <typeparamref name="TModel"/>
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="setTimeField">Установка CreatedAt</param>
        /// <returns></returns>
        public async Task<TEntity> AddAsync<TModel>(TModel model, bool setTimeField = true)
            where TModel : IEntityFillable<TEntity>
        {
            var entity = model.ToEntity();

            if (setTimeField && entity is BaseDateTimedEntity dtEntity)
            {
                dtEntity.CreatedAt = DateTime.Now;
            }

            await DbSet.AddAsync(entity);
            await SaveAsync();

            return entity;
        }

        /// <summary>
        /// Обновление <typeparamref name="TEntity"/> данными из <typeparamref name="TModel"/>
        /// </summary>
        /// <param name="id">ИД сущности</param>
        /// <param name="model">Модель с данными</param>
        /// <param name="save">Сохранять изменения. По умолчанию true, установить false в составе транзакции</param>
        /// <param name="setTimeField">Установка UpdatedAt</param>
        public void Update<TModel>(int id, TModel model, bool save = true, bool setTimeField = true)
            where TModel : IEntityFillable<TEntity>
        {
            Update(e => e.Id == id, model, save, setTimeField);
        }

        /// <summary>
        /// Обновление <typeparamref name="TEntity"/> данными из <typeparamref name="TModel"/>
        /// </summary>
        /// <param name="predicate">Условие выборки</param>
        /// <param name="model">Модель с данными</param>
        /// <param name="save">Сохранять изменения. По умолчанию true, установить false в составе транзакции</param>
        /// <param name="setTimeField">Установка UpdatedAt</param>
        public void Update<TModel>(Expression<Func<TEntity, bool>> predicate, TModel model, bool save = true, bool setTimeField = true)
            where TModel : IEntityFillable<TEntity>
        {
            var entity = DbSetUpdateTransformations.Invoke(DbSet).FirstOrDefault(predicate);

            if (entity == null)
            {
                return;
            }

            model.FillEntity(entity);

            if (setTimeField && entity is BaseDateTimedEntity dtEntity)
            {
                dtEntity.UpdatedAt = DateTime.Now;
            }

            if (save)
            {
                Save();
            }
        }

        /// <summary>
        /// Добавление коллекции <typeparamref name="TModel"/>
        /// </summary>
        public int AddAll<TModel>(IEnumerable<TModel> models)
            where TModel : IEntityFillable<TEntity>
        {
            if (models == null || !models.Any())
            {
                return 0;
            }

            DbSet.AddRange(models.Select(m => m.ToEntity()));

            return Save();
        }

        /// <summary>
        /// Добавление коллекции <typeparamref name="TModel"/>
        /// </summary>
        public int AddAll<TModel>(IEnumerable<TModel> models, out IEnumerable<int> idsAffected)
            where TModel : IEntityFillable<TEntity>
        {
            if (models == null || !models.Any())
            {
                idsAffected = Array.Empty<int>();

                return 0;
            }

            var entities = models
                .Select(m => m.ToEntity())
                .ToList();

            DbSet.AddRange(entities);

            var rowsAffected = Save();

            idsAffected = entities.Select(e => e.Id);

            return rowsAffected;
        }

        /// <summary>
        /// Удалить из БД сразу несколько сущностей типа <typeparamref name="TEntity"/> по запросу <paramref name="selector"/>
        /// </summary>
        /// <param name="selector"></param>
        public int HardDeleteAll(Expression<Func<TEntity, bool>> selector = null)
        {
            selector ??= e => true;
            var entities = DbSet.Where(selector).ToList();
            DbSet.RemoveRange(entities);

            return Save();
        }

        /// <summary>
        /// Мягкое удаление нескольких сущностей
        /// </summary>
        /// <param name="ids">Коллекция ИД сущностей</param>
        public int SoftDeleteAll(IEnumerable<int> ids)
        {
            if (!typeof(TEntity).IsAssignableTo<BaseDateTimedEntity>())
            {
                return 0;
            }
            if (ids == null || !ids.Any())
            {
                return 0;
            }

            var entities = DbSet
                .Where(e => ids.Contains(e.Id))
                .AsEnumerable()
                .OfType<BaseDateTimedEntity>()
                .ToList();

            if (entities.Any())
            {
                entities.ForEach(e => e.DeletedAt ??= DateTime.Now);

                return Save();
            }

            return 0;
        }

        /// <summary>
        /// Полностьб обновим Таблицу
        /// </summary>
        /// <param name="models"></param>
        public void ReplaceTable<TModel>(IEnumerable<TModel> models)
            where TModel : IEntityFillable<TEntity>
        {
            using (var trx = _dbContext.Database.BeginTransaction())
            {
                _dbContext.Database.ExecuteSqlRaw($"TRUNCATE \"{TableName}\"");
                DbSet.AddRange(models.Select(i => i.ToEntity()).ToList());
                Save();
                trx.Commit();
            }
        }

        /// <summary>
        /// Выполняет действие <paramref name="action"/> над коллекцией сущностей
        /// </summary>
        /// <param name="action">Действие</param>
        /// <param name="selector">Фильтр Where. Если не указан - действие выполняется над всеми сущностями</param>
        /// <returns></returns>
        public int Perform(Action<TEntity> action, Expression<Func<TEntity, bool>> selector = null)
        {
            selector ??= e => true;
            var entities = DbSet.Where(selector).ToList();

            if (entities.Any())
            {
                entities.ForEach(action.Invoke);
            }

            return Save();
        }

        /// <inheritdoc cref="StrictAddOrUpdate{TModel}(TModel, Func{IQueryable{TEntity}, IQueryable{TEntity}}, string[])"/>
        public int StrictAddOrUpdate<TModel>(TModel model, params string[] uniquePropNames)
            where TModel : IEntityFillable<TEntity> =>
            StrictAddOrUpdate(model, e => e, uniquePropNames);

        /// <summary>
        /// Добавляет или обновляет сущность со строгим соответствием ИД с возможностью указать уникальные поля
        /// </summary>
        /// <remarks>
        /// Связи могут не подгрузиться, нужно дописать выражение вида <c>dbset => dbset.Include(e => e.Property)</c>
        /// </remarks>
        /// <typeparam name="TModel">Модель данных</typeparam>
        /// <param name="model">Модель данных</param>
        /// <param name="includes">Вызовы .Include() для подтягивания связей</param>
        /// <param name="uniquePropNames">Имена уникальных полей</param>
        public int StrictAddOrUpdate<TModel>(TModel model, Func<IQueryable<TEntity>, IQueryable<TEntity>> includes, params string[] uniquePropNames)
            where TModel : IEntityFillable<TEntity>
        {
            var idProp = typeof(TModel).GetProperty("Id");

            if (idProp == null)
            {
                throw new Exception($"Model of type {typeof(TModel).Name} doesn't contains property \"Id\"");
            }
            if (idProp.PropertyType != typeof(int))
            {
                throw new Exception($"Property \"Id\" of model type {typeof(TModel).Name} is \"{idProp.PropertyType.Name}\", should be \"{typeof(int).Name}\"");
            }

            var id = (int)idProp.GetValue(model);

            var rowsAffected = 0;

            if (uniquePropNames != null && uniquePropNames.Any())
            {
                foreach (var propName in uniquePropNames)
                {
                    var modelUniqueProp = typeof(TModel).GetProperty(propName);
                    var entityUniqueProp = typeof(TEntity).GetProperty(propName);

                    if (modelUniqueProp == null)
                    {
                        throw new Exception($"Model of type {typeof(TModel).Name} doesn't contains property \"{propName}\"");
                    }
                    if (entityUniqueProp == null)
                    {
                        throw new Exception($"Entity of type {typeof(TEntity).Name} doesn't contains property \"{propName}\"");
                    }
                    if (modelUniqueProp.PropertyType != entityUniqueProp.PropertyType)
                    {
                        throw new Exception($"Type of property \"{propName}\" of model type {typeof(TModel).Name} is not the same to entity's of type {typeof(TEntity).Name} (${modelUniqueProp.PropertyType.Name} != ${entityUniqueProp.PropertyType.Name})");
                    }
                }

                var sql = $@"UPDATE ""{Tools.GetEntityTableName(typeof(TEntity))}""
SET {uniquePropNames.Select(e => $"\"{e}\" = gen_random_chars(GREATEST(5, LENGTH(\"{e}\")))").Join(", ")}
WHERE ""Id"" <> {id} AND ({uniquePropNames.Select(e => $"\"{e}\" = @{e}").Join(" OR ")})";

                using (var cmd = _dbContext.Database.GetDbConnection().CreateCommand() as NpgsqlCommand)
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddRange(uniquePropNames.Select(e => new NpgsqlParameter { ParameterName = $"@{e}", Value = typeof(TModel).GetProperty(e)?.GetValue(model) }));
                    cmd.Connection?.SafeOpen();

                    rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected < 0)
                    {
                        rowsAffected = 0;
                    }

                    cmd.Connection?.SafeClose();
                }
            }

            var entity = includes.Invoke(DbSet).FirstOrDefault(e => e.Id == id);

            if (entity == null)
            {
                Add(model);
            }
            else
            {
                model.FillEntity(entity);
            }

            return rowsAffected + Save();
        }

        /// <summary>
        /// Добавляет или обновлят сущность по условию
        /// </summary>
        /// <typeparam name="TModel">Тип модели сущности</typeparam>
        /// <param name="model">Модель</param>
        /// <param name="selector">Условие выборки</param>
        /// <param name="save">Сохранить изменения (false в составе транзакции)</param>
        /// <param name="updateTimeField">Обновить метку времени</param>
        /// <returns></returns>
        public TEntity AddOrUpdate<TModel>(TModel model, Expression<Func<TEntity, bool>> selector, bool save = true, bool updateTimeField = true)
            where TModel : IEntityFillable<TEntity>
        {
            var entity = DbSet.FirstOrDefault(selector);
            var entityPresent = entity != null;
            entity ??= new TEntity();

            model.FillEntity(entity);

            if (updateTimeField && entity.Id > 0 && entity is BaseDateTimedEntity dtEntity)
            {
                dtEntity.UpdatedAt = DateTime.Now;
            }

            if (!entityPresent)
            {
                DbSet.Add(entity);
            }

            if (save)
            {
                Save();
            }

            return entity;
        }

        /// <inheritdoc cref="IDbConnection.CreateCommand()"/>
        public IDbCommand CreateCommand() =>
            _dbContext.Database.GetDbConnection().CreateCommand();

        /// <inheritdoc cref="DatabaseFacade.BeginTransaction()"/>
        public IDbContextTransaction BeginTransaction() =>
            _dbContext.Database.BeginTransaction();

        /// <inheritdoc cref="RelationalDatabaseFacadeExtensions.ExecuteSqlRaw(DatabaseFacade, string, object[])"/>
        public int ExecuteSqlRaw(string sql) =>
            _dbContext.Database.ExecuteSqlRaw(sql);

        /// <inheritdoc cref="DatabaseExtensions.FromSqlRaw{T}(DatabaseFacade, string, NpgsqlParameter[])"/>
        public IEnumerable<T> FromSqlRaw<T>(string sql, params NpgsqlParameter[] args) =>
            _dbContext.Database.FromSqlRaw<T>(sql, args);

        /// <inheritdoc cref="DatabaseExtensions.FromSqlRaw{T1, T2}(DatabaseFacade, string, NpgsqlParameter[])"/>
        public IEnumerable<Tuple<T1, T2>> FromSqlRaw<T1, T2>(string sql, params NpgsqlParameter[] args) =>
            _dbContext.Database.FromSqlRaw<T1, T2>(sql, args);

        /// <inheritdoc cref="DatabaseExtensions.FromSqlRaw{T1, T2, T3}(DatabaseFacade, string, NpgsqlParameter[])"/>
        public IEnumerable<Tuple<T1, T2, T3>> FromSqlRaw<T1, T2, T3>(string sql, params NpgsqlParameter[] args) =>
            _dbContext.Database.FromSqlRaw<T1, T2, T3>(sql, args);

        public async ValueTask DisposeAsync()
        {
            await _dbContext.DisposeAsync();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
