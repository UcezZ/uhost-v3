﻿using System;
using System.Collections.Generic;
using Uhost.Core.Models.User;
using Entity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Services.User
{
    public interface IUserService : IDisposable, IAsyncDisposable
    {
        Entity Add(UserCreateModel model);
        Entity Add(UserRegisterModel model);
        bool CheckRoleIds(IEnumerable<int> ids, out int invalid);
        void Delete(int id);
        bool Exists(string login, int excludedId = 0);
        UserAccessModel GetAccessData(int id);
        object GetAllPaged(UserQueryModel query);
        UserViewModel GetOne(int id);
        void Update(int id, UserPasswordUpdateModel model);
        void Update(int id, UserCreateModel model);
        void Update(int id, UserBaseModel model);
    }
}
