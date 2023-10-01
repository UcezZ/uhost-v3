using System;
using System.Net.Http;
using System.Threading.Tasks;
using Uhost.Core.Models.RestClient;

namespace Uhost.Core.Services.RestClient
{
    public interface IRestClientService : IDisposable
    {
        BaseRestJsonResponseModel<TDataModel> GetJsonResponseModel<TDataModel>(BaseRestQueryModel query) where TDataModel : class;
        Task<BaseRestJsonResponseModel<TDataModel>> GetJsonResponseModelAsync<TDataModel>(BaseRestQueryModel query) where TDataModel : class;
        HttpResponseMessage GetResponse(BaseRestQueryModel query);
        TResponseModel GetResponseModel<TResponseModel>(BaseRestQueryModel query) where TResponseModel : BaseRestResponseModel, new();
        Task<TResponseModel> GetResponseModelAsync<TResponseModel>(BaseRestQueryModel query) where TResponseModel : BaseRestResponseModel, new();
    }
}
