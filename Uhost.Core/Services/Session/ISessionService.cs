using System;
using System.Threading.Tasks;
using Uhost.Core.Models;
using Uhost.Core.Models.Session;

namespace Uhost.Core.Services.Session
{
    public interface ISessionService
    {
        Task<PagerResultModel<SessionViewModel>> GetAllPaged(SessionQueryModel query);
        Task<bool> Terminate(Guid sessionGuid);
    }
}