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
        public DisposableFileStreamResult(Stream fileStream, string contentType) : base(fileStream, contentType) { }

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
