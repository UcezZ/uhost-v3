using Microsoft.AspNetCore.Mvc;
using System;

namespace Uhost.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class IgnoreIfVariableUndefinedAttribute : ApiExplorerSettingsAttribute
    {
        public IgnoreIfVariableUndefinedAttribute(string name) : base()
        {
            IgnoreApi = Environment.GetEnvironmentVariable(name) == null;
        }
    }
}
