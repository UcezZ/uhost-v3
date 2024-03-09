using Microsoft.AspNetCore.Mvc;

namespace Uhost.Web.Attributes
{
    public class BigFileUploadAttribute : RequestSizeLimitAttribute
    {
        private const long _size = 4L * 1024L * 1024L * 1024L;

        public BigFileUploadAttribute() : base(_size) { }
    }
}
