using System;
using System.Collections.Generic;
using Uhost.Core.Models.User;
using Entity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Services.User
{
    public interface IUserService : IDisposable
    {
        Entity Add(UserCreateModel model);
        Entity Add(UserRegisterModel model);
        Entity Auth(UserLoginQueryModel query);
        bool CheckRoleIds(IEnumerable<int> ids, out int invalid);
        void Delete(int id);
        void DeleteAvatar(int userId = 0);
        bool Exists(string login, string email, int excludedId = 0);
        UserAccessModel GetAccessData(int id);
        object GetAllPaged(UserQueryModel query);
        UserViewModel GetOne(int id);
        UserViewModel GetOne(string login);
        void Update(int id, UserPasswordUpdateModel model);
        void Update(int id, UserCreateModel model);
        void Update(int id, UserSelfUpdateModel model);
        int UpdateLastVisitAt(int id);
        string UploadAvatar(UserUpdateAvatarModel model, int userId = 0);
    }
}
