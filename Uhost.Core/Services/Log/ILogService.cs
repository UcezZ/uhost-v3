using System;
using Uhost.Core.Models;
using Uhost.Core.Models.Log;
using static Uhost.Core.Data.Entities.Log;

namespace Uhost.Core.Services.Log
{
    public interface ILogService : IDisposable
    {
        /// <summary>
        /// Добавление события в лог
        /// </summary>
        /// <param name="ev">Тип события</param>
        /// <param name="data">Данные</param>
        /// <param name="userId"></param>
        /// <param name="writeToStdOut">Вывод в STDOUT</param>
        void Add(Events ev, object data = null, int userId = default, bool writeToStdOut = false);

        /// <summary>
        /// Получение логов по запросу
        /// </summary>
        /// <param name="query">Параметры запроса</param>
        /// <returns></returns>
        PagerResultModel<LogViewModel> GetAllPaged(LogQueryModel query);
    }
}
