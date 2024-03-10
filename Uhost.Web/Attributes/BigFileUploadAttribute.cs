using Microsoft.AspNetCore.Mvc;

namespace Uhost.Web.Attributes
{
    public class BigFileUploadAttribute : RequestSizeLimitAttribute
    {
        public const long Size = 8L * 1024L * 1024L * 1024L;

        public BigFileUploadAttribute() : base(Size) { }
    }
}
