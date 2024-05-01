using System;
using System.Collections.Generic;
using Uhost.Core.Models;
using Uhost.Core.Models.Log;
using static Uhost.Core.Data.Entities.Log;

namespace Uhost.Core.Services.Log
{
    public interface ILogService : IDisposable
    {
        /// <summary>
        /// Все описания событий
        /// </summary>
        IDictionary<Events, string> AllEvents { get; }

        /// <summary>
        /// Добавление события в лог
        /// </summary>
        /// <param name="ev">Тип события</param>
        /// <param name="data">Данные</param>
        /// <param name="writeToStdOut">Вывод в STDOUT</param>
        void Add(Events ev, object data = null, bool writeToStdOut = false);

        /// <summary>
        /// Получение логов по запросу
        /// </summary>
        /// <param name="query">Параметры запроса</param>
        /// <returns></returns>
        PagerResultModel<LogViewModel> GetAllPaged(LogQueryModel query);
    }
}
