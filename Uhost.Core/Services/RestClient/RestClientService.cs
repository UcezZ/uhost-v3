using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Uhost.Core.Models.RestClient;
using static System.Console;

namespace Uhost.Core.Services.RestClient
{
    /// <summary>
    /// Универсальный REST клиент
    /// </summary>
    public sealed class RestClientService : IRestClientService
    {
        private readonly HttpClient _httpClient;

        public RestClientService()
        {
            _httpClient = new HttpClient();
        }

        public HttpResponseMessage GetResponse(BaseRestQueryModel query)
        {
            var request = query.ToHttpRequest();

            return _httpClient.Send(request);
        }

        /// <summary>
        /// Получает общую модель ответа
        /// </summary>
        /// <typeparam name="TResponseModel">Целевой тип производной модели ответа</typeparam>
        /// <param name="query">Параметры запроса</param>
        /// <returns></returns>
        public TResponseModel GetResponseModel<TResponseModel>(BaseRestQueryModel query) where TResponseModel : BaseRestResponseModel, new()
        {
            var request = query.ToHttpRequest();
            var response = _httpClient.Send(request);
            var model = new TResponseModel
            {
                HttpResponse = response
            };

            try
            {
                var task = response.Content.ReadAsStringAsync();
                task.Wait();
                model.Contents = task.Result;
            }
            catch (Exception e)
            {
                model.Exception = e;
            }

            model.Success = model.Exception == null;

            return model;
        }

        /// <summary>
        /// Получает ответ JSON, заполняет данными целевую модель
        /// </summary>
        /// <typeparam name="TDataModel">Целевая модель ответа REST API</typeparam>
        /// <param name="query">Параметры запроса</param>
        public BaseRestJsonResponseModel<TDataModel> GetJsonResponseModel<TDataModel>(BaseRestQueryModel query) where TDataModel : class
        {
            var model = GetResponseModel<BaseRestJsonResponseModel<TDataModel>>(query);

            if (!model.Success)
            {
                return model;
            }

            try
            {
                var jToken = JToken.Parse(model.Contents);
                model.Payload = jToken.ToObject<TDataModel>();
            }
            catch (Exception e)
            {
                if (model.Contents != null)
                {
                    Out.WriteLine(model.Contents);
                }

                model.Exception = e;
            }

            model.Success = model.Exception == null;

            return model;
        }

        /// <inheritdoc cref="GetResponseModel{TResponseModel}(BaseRestQueryModel)"/>
        public async Task<TResponseModel> GetResponseModelAsync<TResponseModel>(BaseRestQueryModel query) where TResponseModel : BaseRestResponseModel, new()
        {
            var request = query.ToHttpRequest();
            var response = await _httpClient.SendAsync(request);
            var model = new TResponseModel
            {
                HttpResponse = response
            };

            try
            {
                model.Contents = await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                model.Exception = e;
            }

            model.Success = model.Exception == null;

            return model;
        }

        /// <inheritdoc cref="GetJsonResponseModel{T}(BaseRestQueryModel)"/>
        public async Task<BaseRestJsonResponseModel<TDataModel>> GetJsonResponseModelAsync<TDataModel>(BaseRestQueryModel query) where TDataModel : class
        {
            var model = await GetResponseModelAsync<BaseRestJsonResponseModel<TDataModel>>(query);

            if (!model.Success)
            {
                return model;
            }

            try
            {
                var jToken = JToken.Parse(model.Contents);
                model.Payload = jToken.ToObject<TDataModel>();
            }
            catch (Exception e)
            {
                if (model.Contents != null)
                {
                    await Out.WriteLineAsync(model.Contents);
                }

                model.Exception = e;
            }

            model.Success = model.Exception == null;

            return model;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
