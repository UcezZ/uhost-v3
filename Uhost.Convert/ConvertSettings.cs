using Uhost.Core;

namespace Uhost.Convert
{
    static class ConvertSettings
    {

        static ConvertSettings() => CoreSettings.Load(typeof(ConvertSettings));
    }
}
