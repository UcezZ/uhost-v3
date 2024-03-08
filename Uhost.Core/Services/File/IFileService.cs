using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uhost.Core.Models;
using Uhost.Core.Models.File;
using Entity = Uhost.Core.Data.Entities.File;
using QueryModel = Uhost.Core.Models.File.FileQueryModel;

namespace Uhost.Core.Services.File
{
    public interface IFileService : IDisposable
    {
        Entity Add(FileUploadModel model);
        Entity Add(IFormFile file, Entity.FileTypes? type = null, string dynName = null, int? dynId = null);
        Entity Add(FileInfo file, string name = null, string mime = null, Entity.FileTypes? type = null, string dynName = null, int? dynId = null);
        Entity Add(Stream data, string name, string mime = null, Entity.FileTypes? type = null, string dynName = null, int? dynId = null);
        Entity Add(IFormFile file, Entity.FileTypes? type = null, Type dynType = null, int? dynId = null);
        Entity Add(FileInfo file, string name = null, string mime = null, Entity.FileTypes? type = null, Type dynType = null, int? dynId = null);
        void Delete(int id, bool deleteFile = false);
        IQueryable<TModel> GetAll<TModel>(QueryModel query) where TModel : BaseModel<Entity>, new();
        object GetAllPaged<TModel>(QueryModel query) where TModel : BaseModel<Entity>, new();
        IQueryable<TFileModel> GetByDynEntity<TFileModel>(IEnumerable<int> ids, Type dynEntity, params Entity.FileTypes[] types) where TFileModel : BaseModel<Entity>, new();
        IQueryable<TFileModel> GetByDynEntity<TFileModel>(int id, Type dynEntity, params Entity.FileTypes[] types) where TFileModel : BaseModel<Entity>, new();
        TModel GetOne<TModel>(int id) where TModel : BaseModel<Entity>, new();
    }
}
