using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sentry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Uhost.Core.Models.File;
using Uhost.Core.Repositories;
using Uhost.Core.Services.Log;
using static Uhost.Core.Data.Entities.Log;
using Entity = Uhost.Core.Data.Entities.File;
using QueryModel = Uhost.Core.Models.File.FileQueryModel;

namespace Uhost.Core.Services.File
{
    public sealed class FileService : BaseService, IFileService
    {
        private readonly FileRepository _repo;
        private readonly ILogService _log;

        public FileService(IDbContextFactory<PostgreSqlDbContext> factory, IServiceProvider provider, ILogService log) : base(factory, provider)
        {
            _repo = new FileRepository(_dbContext);
            _log = log;
        }

        /// <summary>
        /// Создаёт временный файл
        /// </summary>
        /// <returns></returns>
        private static FileStream CreateTempFile()
        {
            try
            {
                var temp = Path.GetFullPath(Path.Combine(Path.GetTempPath(), $"upload_{Guid.NewGuid()}"));

                Tools.MakePath(temp);

                return new FileStream(temp, FileMode.Create, FileAccess.ReadWrite);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);

                var temp = Path.GetFullPath(Path.Combine(Path.GetFullPath("tmp"), $"upload_{Guid.NewGuid()}"));

                Tools.MakePath(temp);

                return new FileStream(temp, FileMode.Create, FileAccess.ReadWrite);
            }
        }

        /// <summary>
        /// Размещает файл в ФС
        /// </summary>
        /// <param name="entity">Сущность файла</param>
        /// <param name="data">Поток данных</param>
        /// <returns></returns>
        private bool StoreFile(Entity entity, Stream data)
        {
            var file = new FileInfo(entity.GetPath());

            try
            {
                if (!file.Directory.Exists)
                {
                    file.Directory.Create();
                }

                var doCopy = true;

                if (data is FileStream fileStream)
                {
                    var fileInfo = new FileInfo(fileStream.Name);

                    try
                    {
                        fileStream.Close();
                        fileInfo.MoveTo(file.FullName, true);
                        doCopy = false;
                    }
                    catch
                    {
                        data = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
                    }
                }

                if (doCopy)
                {
                    using (var stream = file.OpenWrite())
                    {
                        stream.SetLength(0);
                        data.Position = 0;
                        data.CopyTo(stream);
                    }
                }

                try
                {
                    file.CreationTime = entity.CreatedAt;
                    file.LastAccessTime = entity.UpdatedAt ?? entity.CreatedAt;
                    file.LastWriteTime = entity.UpdatedAt ?? entity.CreatedAt;
                }
                catch { }

                return true;
            }
            catch (Exception e)
            {
                _log.Add(Events.FileStoreFail, new
                {
                    File = new
                    {
                        file.FullName,
                        file.Length,
                        file.CreationTime,
                        file.Exists
                    },
                    Exception = e.ToDetailedDataObject()
                });

                SentrySdk.CaptureException(e);

                return false;
            }
        }

        public IQueryable<TModel> GetAll<TModel>(QueryModel query) where TModel : BaseModel<Entity>, new()
        {
            return _repo.GetAll<TModel>(query);
        }

        public object GetAllPaged<TModel>(QueryModel query) where TModel : BaseModel<Entity>, new()
        {
            var pager = GetAll<TModel>(query).CreatePager(query);

            return pager.Paginate();
        }

        public TModel GetOne<TModel>(int id) where TModel : BaseModel<Entity>, new()
        {
            return GetAll<TModel>(new QueryModel { Id = id }).FirstOrDefault();
        }

        public IQueryable<TFileModel> GetByDynEntity<TFileModel>(int id, Type dynEntity, params Entity.FileTypes[] types) where TFileModel : BaseModel<Entity>, new()
        {
            var query = new QueryModel
            {
                DynId = id,
                DynName = dynEntity.Name,
                Types = types.Any() ? types.Select(e => e.ToString()) : null,
                SortBy = nameof(Entity.SortBy.CreatedAt),
                SortDirection = nameof(BaseQueryModel.SortDirections.Desc)
            };

            return GetAll<TFileModel>(query);
        }

        public IQueryable<TFileModel> GetByDynEntity<TFileModel>(IEnumerable<int> ids, Type dynEntity, params Entity.FileTypes[] types) where TFileModel : BaseModel<Entity>, new()
        {
            var query = new QueryModel
            {
                DynIds = ids,
                DynName = dynEntity.Name,
                Types = types.Any() ? types.Select(e => e.ToString()) : null,
                SortBy = nameof(Entity.SortBy.CreatedAt),
                SortDirection = nameof(BaseQueryModel.SortDirections.Desc)
            };

            return GetAll<TFileModel>(query);
        }

        public Entity Add(FileUploadModel model)
        {
            return Add(model.File, model.TypeParsed, model.DynName, model.DynId);
        }

        public Entity Add(IFormFile file, Entity.FileTypes? type = null, Type dynType = null, int? dynId = null)
        {
            return Add(file, type, dynType?.Name, dynId);
        }

        public Entity Add(IFormFile file, Entity.FileTypes? type = null, string dynName = null, int? dynId = null)
        {
            var temp = CreateTempFile();

            try
            {
                using (var upload = file.OpenReadStream())
                {
                    upload.CopyTo(temp);
                }

                temp.Position = 0;

                var entity = Add(temp, file.FileName, file.ContentType, type, dynName, dynId);

                _log.Add(Events.FileUploaded, file);

                return entity;
            }
            catch (Exception e)
            {
                _log.Add(Events.FileUploadFail, new
                {
                    File = file,
                    Exception = e.ToDetailedDataObject()
                });

                SentrySdk.CaptureException(e);

                return null;
            }
            finally
            {
                var name = temp.Name;
                temp.Dispose();

                try
                {
                    System.IO.File.Delete(name);
                }
                catch (Exception e)
                {
                    SentrySdk.CaptureException(e);
                }
            }
        }

        public Entity Add(FileInfo file, string name = null, string mime = null, Entity.FileTypes? type = null, Type dynType = null, int? dynId = null)
        {
            return Add(file, name: name, mime: mime, type: type, dynName: dynType?.Name, dynId: dynId);
        }

        public Entity Add(FileInfo file, string name = null, string mime = null, Entity.FileTypes? type = null, string dynName = null, int? dynId = null)
        {
            using (var stream = file.OpenRead())
            {
                return Add(stream, name ?? file.Name, mime, type, dynName, dynId);
            }
        }

        public Entity Add(Stream data, string name, string mime = null, Entity.FileTypes? type = null, string dynName = null, int? dynId = null)
        {
            var digest = data
                .ComputeHash(HasherExtensions.EncryptionMethod.MD5)
                .ToHexString();

            var model = new FileCreateModel
            {
                Name = name,
                DynId = dynId,
                DynName = dynName,
                Mime = mime,
                Size = data.Length,
                Type = type ?? Entity.FileTypes.Other,
                UserId = TryGetUserId(out var userId) ? userId : null,
                Digest = digest
            };

            using (var trx = _repo.BeginTransaction())
            {
                var entity = _repo.Add(model);

                if (StoreFile(entity, data))
                {
                    trx.Commit();

                    return entity;
                }
                else
                {
                    trx.Rollback();

                    return null;
                }
            }
        }

        public void Delete(int id, bool deleteFile = false)
        {
            if (_repo.FindEntity(id, out var entity))
            {
                if (deleteFile)
                {
                    var file = new FileInfo(entity.GetPath());
                    file.TryDeleteIfExists();
                }

                _repo.SoftDelete(id);
            }
        }
    }
}
