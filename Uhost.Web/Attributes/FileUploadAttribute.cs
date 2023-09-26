using System;

namespace Uhost.Web.Attributes
{
    /// <summary>
    /// Помечает, что метод контроллера может принимать файлы параметрами метода
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class FileUploadAttribute : Attribute { }
}
