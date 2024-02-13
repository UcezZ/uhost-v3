using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Uhost.Core.Extensions;
using Uhost.Web.Properties;
using static Uhost.Core.Data.Entities.Right;

namespace Uhost.Web.Common
{

    // Класс содержит ф-ии которые помогают софрмировать ответ в соответсвии с принятым стандартом кодирования
    public static class ResponseHelper
    {
        public const string СontentTypeJson = "application/json;charset=utf-8";

        // Стандартный ответ
        private static IActionResult MakeStandartAnswer(object data, HttpStatusCode status = HttpStatusCode.OK)
        {
            return new ContentResult()
            {
                ContentType = СontentTypeJson,
                Content = data.ToJson(),
                StatusCode = (int)status,
            };
        }

        /// <summary>
        /// Стандартная форма полоительного ответа
        /// </summary>
        public static object MakeSuccessData(object data)
        {
            return new
            {
                Success = true,
                Result = data
            };
        }

        /// <summary>
        /// Стандартная форма негативного ответа
        /// </summary>
        public static object MakeErrorData(object data)
        {
            return new
            {
                Success = false,
                Errors = data
            };
        }

        // Успешный ответ по принятому формату
        public static IActionResult Success(object data = null, HttpStatusCode status = HttpStatusCode.OK)
        {
            data ??= new { };

            return MakeStandartAnswer(
                MakeSuccessData(data),
                status
            );
        }

        // Ответ с ошибками
        public static IActionResult Error(object data, HttpStatusCode status = HttpStatusCode.UnprocessableEntity)
        {
            return MakeStandartAnswer(
                MakeErrorData(data),
                status
            );
        }

        // Ответ с одном сообщением
        public static IActionResult ErrorMessage(string errorKey, string errorMessage, HttpStatusCode status = HttpStatusCode.UnprocessableEntity)
        {
            var errorObj = new Dictionary<string, string[]>
            {
                [errorKey] = new string[] { errorMessage }
            };

            return Error(errorObj, status);
        }

        // Ответ, ошибка валидации, с форматированной строкой
        public static IActionResult ErrorMessageValidationFmt(string errorKey, string errorMessageFmt, params object[] vals)
        {
            string errMsg = string.Format(errorMessageFmt, vals);
            var errorObj = new Dictionary<string, string[]>
            {
                [errorKey] = new string[] { errMsg }
            };

            return Error(errorObj, HttpStatusCode.UnprocessableEntity);
        }

        // Ответ, не найдено, с форматированной строкой
        public static IActionResult ErrorMessageNotFoundFmt(string errorKey, string errorMessageFmt, params object[] vals)
        {
            string errMsg = string.Format(errorMessageFmt, vals);
            var errorObj = new Dictionary<string, string[]>
            {
                [errorKey] = new string[] { errMsg }
            };

            return Error(errorObj, HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Возвращает ответ с форматированной строкой и указанным кодом ошибки
        /// </summary>
        /// <param name="httpStatusCode">Код HTTP</param>
        /// <param name="errorKey">Поле с неправильным значением</param>
        /// <param name="errorMessageFmt">Строка сообщения с форматированием</param>
        /// <param name="vals">Аргументы форматирования</param>
        /// <returns></returns>
        public static IActionResult ErrorMessageFmt(HttpStatusCode httpStatusCode, string errorKey, string errorMessageFmt, params object[] vals)
        {
            string errMsg = string.Format(errorMessageFmt, vals);
            var errorObj = new Dictionary<string, string[]>
            {
                [errorKey] = new string[] { errMsg }
            };

            return Error(errorObj, httpStatusCode);
        }

        /// <summary>
        /// Возвращает ответ с форматированной строкой и кодом 422
        /// </summary>
        /// <param name="errorKey">Поле с неправильным значением</param>
        /// <param name="errorMessageFmt">Строка сообщения с форматированием</param>
        /// <param name="vals">Аргументы форматирования</param>
        /// <returns></returns>
        public static IActionResult ErrorMessageFmt(string errorKey, string errorMessageFmt, params object[] vals)
        {
            return ErrorMessageFmt(HttpStatusCode.UnprocessableEntity, errorKey, errorMessageFmt, vals);
        }

        /// <summary>
        /// Возвращает ответ 403 с сообщением об отсутствии права
        /// </summary>
        /// <param name="missingRight">Отсутствующее право</param>
        public static IActionResult ErrorForbidden(params Rights?[] missingRight)
        {
            return MakeStandartAnswer(
                new
                {

                    Success = false,
                    Errors = new[]
                    {
                        string.Format(ApiStrings.Right_Error_ShouldHaveAny, string.Join(", ", missingRight.Select(e => e.Translate())))
                    }
                },
                HttpStatusCode.Forbidden);
        }
    }
}
