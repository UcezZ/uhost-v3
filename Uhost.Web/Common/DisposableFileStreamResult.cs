using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace Uhost.Web.Common
{
    /// <summary>
    /// То же самое, что <see cref="FileStreamResult"/>, но уничтожит поток <see cref="FileStreamResult.FileStream"/> после ответа
    /// </summary>
    public class DisposableFileStreamResult : FileStreamResult
    {
        /// <summary>
        /// <inheritdoc cref="DisposableFileStreamResult"/>
        /// </summary>
        /// <param name="fileStream">Поток данных</param>
        /// <param name="contentType">MIME</param>
        public DisposableFileStreamResult(Stream fileStream, string contentType) : base(fileStream, contentType) { }

        /// <summary>
        /// <inheritdoc cref="DisposableFileStreamResult"/>
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="contentType">MIME</param>
        public DisposableFileStreamResult(byte[] data, string contentType) : this(new MemoryStream(data), contentType) { }

        public override void ExecuteResult(ActionContext context)
        {
            using (FileStream)
            {
                base.ExecuteResult(context);
            }
        }

        public async override Task ExecuteResultAsync(ActionContext context)
        {
            using (FileStream)
            {
                await base.ExecuteResultAsync(context);
            }
        }
    }
}
