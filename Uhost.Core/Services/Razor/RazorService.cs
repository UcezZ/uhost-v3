using RazorLight;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Uhost.Core.Services.Razor
{
    public class RazorService : IRazorService
    {
        public enum Templates
        {
            Registration
        }

        private readonly RazorLightEngine _engine;
        private static readonly string _templatesBasePath = Path.Combine(AppContext.BaseDirectory, "Templates");

        public RazorService()
        {
            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(_templatesBasePath)
                .UseMemoryCachingProvider()
                .Build();
        }

        /// <summary>
        /// Рендерит шаблон с использованием соответствующей модели
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public string RenderToString<T>(string viewName, T model)
        {
            var path = Path.Combine(_templatesBasePath, viewName);

            if (!System.IO.File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            var task = _engine.CompileRenderAsync(viewName, model);
            task.Wait();

            return task.Result;
        }

        /// <summary>
        /// Рендерит шаблон с использованием соответствующей модели
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public string RenderToString<T>(Templates template, T model)
        {
            return RenderToString(Path.Combine(_templatesBasePath, template.ToString()), model);
        }

        /// <summary>
        /// Рендерит шаблон с использованием соответствующей модели
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> RenderToStringAsync<T>(string viewName, T model)
        {
            var path = Path.Combine(_templatesBasePath, viewName);

            if (!System.IO.File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            return await _engine.CompileRenderAsync(viewName, model);
        }

        /// <summary>
        /// Рендерит шаблон с использованием соответствующей модели
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> RenderToStringAsync<T>(Templates template, T model)
        {
            return await RenderToStringAsync(Path.Combine(_templatesBasePath, template.ToString()), model);
        }
    }
}
