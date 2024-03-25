using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Uhost.Core.Data;
using Uhost.Core.Models;

namespace Uhost.Tests.Extensions
{
    internal static class ControllerExtensions
    {
        /// <summary>
        /// Авторизация контроллера. Данные авторизации получить через <see cref="Web.Services.Auth.IAuthService.CreateClaims(int)"/>
        /// </summary>
        /// <param name="controller">Контроллер</param>
        /// <param name="claims">Данные авторизации</param>
        public static void Authorize(this Controller controller, ClaimsPrincipal claims)
        {
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = claims
                }
            };
        }

        /// <summary>
        /// Сброс авторизации контроллера
        /// </summary>
        /// <param name="controller">Контроллер</param>
        public static void ResetAuthorization(this Controller controller)
        {
            controller.ControllerContext = new ControllerContext();
        }

        /// <inheritdoc cref="ValidateModel{TQueryModel}(IServiceProvider, TQueryModel, out ICollection{ValidationResult})"/>
        public static bool ValidateModel<TQueryModel>(this IServiceProvider provider, TQueryModel model)
           where TQueryModel : BaseQueryModel =>
           provider.ValidateModel(model, out _);

        /// <summary>
        /// Валидация модели запроса
        /// </summary>
        /// <typeparam name="TQueryModel">Тип модели запроса</typeparam>
        /// <param name="provider">Провайдер сервисов</param>
        /// <param name="model">Модель запроса</param>
        /// <param name="errors">Коллекция ошибок валидации</param>
        /// <returns>Валидность модели</returns>
        public static bool ValidateModel<TQueryModel>(this IServiceProvider provider, TQueryModel model, out ICollection<ValidationResult> errors)
            where TQueryModel : BaseQueryModel
        {
            errors = new List<ValidationResult>();
            var context = new ValidationContext(model, provider, items: null);
            var isValid = Validator.TryValidateObject(model, context, errors, true);

            return isValid;
        }

        /// <inheritdoc cref="ValidateModel{TModel, TEntity}(IServiceProvider, TModel, out ICollection{ValidationResult})"/>
        public static bool ValidateModel<TModel, TEntity>(this IServiceProvider provider, TModel model)
            where TModel : IEntityFillable<TEntity>
            where TEntity : BaseEntity, new() =>
            provider.ValidateModel<TModel, TEntity>(model, out _);

        /// <summary>
        /// Валидация модели сущности
        /// </summary>
        /// <typeparam name="TModel">Тип модели сущности</typeparam>
        /// <typeparam name="TEntity">Тип сущности</typeparam>
        /// <param name="provider">Провайдер сервисов</param>
        /// <param name="model">Модель сущности</param>
        /// <param name="errors">Коллекция ошибок валидации</param>
        /// <returns>Валидность модели</returns>
        public static bool ValidateModel<TModel, TEntity>(this IServiceProvider provider, TModel model, out ICollection<ValidationResult> errors)
            where TModel : IEntityFillable<TEntity>
            where TEntity : BaseEntity, new()
        {
            errors = new List<ValidationResult>();
            var context = new ValidationContext(model, provider, items: null);
            var isValid = Validator.TryValidateObject(model, context, errors, true);

            return isValid;
        }
    }
}
