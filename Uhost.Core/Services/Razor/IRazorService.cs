using System.Threading.Tasks;

namespace Uhost.Core.Services.Razor
{
    public interface IRazorService
    {
        string RenderToString<T>(string viewName, T model);
        string RenderToString<T>(RazorService.Templates template, T model);
        Task<string> RenderToStringAsync<T>(string viewName, T model);
        Task<string> RenderToStringAsync<T>(RazorService.Templates template, T model);
    }
}
