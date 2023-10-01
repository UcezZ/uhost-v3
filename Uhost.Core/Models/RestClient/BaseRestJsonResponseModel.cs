namespace Uhost.Core.Models.RestClient
{
    /// <summary>
    /// Базовая модель ответа REST API c JSON содержимым
    /// </summary>
    /// <typeparam name="TDataModel">Целевой тип модели ответа</typeparam>
    public class BaseRestJsonResponseModel<TDataModel> : BaseRestResponseModel where TDataModel : class
    {
        /// <summary>
        /// Содержимое ответа в виде объекта
        /// </summary>
        public TDataModel Payload { get; set; }
    }
}
