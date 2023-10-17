using Microsoft.AspNetCore.Http;
using Sentry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Uhost.Core.Models.File;
using Uhost.Core.Repositories;
using Uhost.Core.Services.Log;
using static Uhost.Core.Data.Entities.Log;
using Entity = Uhost.Core.Data.Entities.File;
using QueryModel = Uhost.Core.Models.File.FileQueryModel;

namespace Uhost.Core.Services.FileService
{
    public class FileService : BaseService, IFileService
    {
        private readonly FileRepository _repo;
        private readonly ILogService _log;
        private readonly IHttpContextAccessor _contextAccessor;

        public FileService(IServiceProvider provider, PostgreSqlDbContext dbContext, ILogService log) : base(dbContext)
        {
            _repo = new FileRepository(_dbContext);
            _log = log;
            _contextAccessor = provider.GetService<IHttpContextAccessor>();
        }

        private static FileStream CreateTempFile()
        {
            try
            {
                var tempDir = Path.GetTempPath();

                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                var temp = Path.GetFullPath(Path.Combine(tempDir, $"upload_{Guid.NewGuid()}"));

                return new FileStream(temp, FileMode.Create, FileAccess.ReadWrite);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);

                var tempDir = Path.GetFullPath("tmp");

                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                var temp = Path.GetFullPath(Path.Combine(tempDir, $"upload_{Guid.NewGuid()}"));

                return new FileStream(temp, FileMode.Create, FileAccess.ReadWrite);
            }
        }

        private bool StoreFile(Entity entity, Stream data)
        {
            var file = new FileInfo(Path.Combine(CoreSettings.FileStoragePath, entity.Token[0..2], entity.Token[2..4], entity.Token[4..], entity.Name));

            try
            {
                if (!file.Directory.Exists)
                {
                    file.Directory.Create();
                }

                using (var stream = file.OpenWrite())
                {
                    stream.SetLength(0);
                    data.Position = 0;
                    data.CopyTo(stream);
                }

                file.CreationTime = entity.CreatedAt;
                file.LastAccessTime = entity.CreatedAt;
                file.LastWriteTime = entity.CreatedAt;

                return true;
            }
            catch (Exception e)
            {
                _log.Add(Events.FileStoreFail, new
                {
                    File = file,
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

        public IQueryable<TFileModel> GetByDynEntity<TFileModel>(IEnumerable<int> ids, Type dynEntity, params Entity.Types[] types) where TFileModel : BaseModel<Entity>, new()
        {
            var query = new QueryModel
            {
                DynIds = ids,
                DynName = dynEntity.Name,
                Types = types.Any() ? types.Select(e => e.ToString()) : null
            };

            return GetAll<TFileModel>(query);
        }

        public Entity Add(FileUploadModel model)
        {
            return Add(model.File, model.TypeParsed ?? Entity.Types.Other, model.DynName, model.DynId);
        }

        public Entity Add(IFormFile file, Entity.Types type = Entity.Types.Other, string dynName = null, int? dynId = null)
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
                    File.Delete(name);
                }
                catch (Exception e)
                {
                    SentrySdk.CaptureException(e);
                }
            }
        }

        public Entity Add(FileInfo file, string mime = null, Entity.Types type = Entity.Types.Other, string dynName = null, int? dynId = null)
        {
            using (var stream = file.OpenRead())
            {
                return Add(stream, file.Name, mime, type, dynName, dynId);
            }
        }

        public Entity Add(Stream data, string name, string mime = null, Entity.Types type = Entity.Types.Other, string dynName = null, int? dynId = null)
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
                Size = (int)data.Length,
                Type = type,
                UserId = _contextAccessor?.HttpContext?.User != null && _contextAccessor.HttpContext.User.TryGetUserId(out var userId) ? userId : null,
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
                    var file = new FileInfo(Path.Combine(CoreSettings.FileStoragePath, entity.Token[0..2], entity.Token[2..4], entity.Token[4..], entity.Name));

                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                _repo.SoftDelete(id);
            }
        }
    }
}
