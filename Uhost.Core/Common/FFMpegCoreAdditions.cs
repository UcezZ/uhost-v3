using FFMpegCore;
using FFMpegCore.Helpers;
using Instances;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Uhost.Core.Common
{
    public static class FFMpegCoreAdditions
    {
        private static readonly Regex _sizeOutputRegex = new Regex(@"^(\d+,?)+$");

        /// <summary>
        /// Правильно определяет разрешение с учётом поворота кадра
        /// </summary>
        /// <param name="fileName">Путь к файлу</param>
        /// <param name="size">Размер</param>
        /// <param name="options">Параметры ffmpeg</param>
        /// <returns></returns>
        public static bool TryGetSize(string fileName, out Size size, FFOptions options = null)
        {
            fileName = Path.GetFullPath(fileName);

            if (!File.Exists(fileName))
            {
                size = default;
                return false;
            }

            options ??= GlobalFFOptions.Current;

            FFProbeHelper.VerifyFFProbeExists(options);

            var startInfo = new ProcessStartInfo(GlobalFFOptions.GetFFProbeBinaryPath(options), $"-v error -select_streams v:0 -show_entries stream=width,height:stream_tags=rotate -of csv=p=0 \"{fileName}\"")
            {
                StandardOutputEncoding = options.Encoding,
                StandardErrorEncoding = options.Encoding,
                WorkingDirectory = options.WorkingDirectory
            };
            var instance = new Instance(startInfo);

            var exitCode = instance.BlockUntilFinished();

            if (exitCode == 0)
            {
                var result = instance.ErrorData.Concat(instance.OutputData)
                    .FirstOrDefault(_sizeOutputRegex.IsMatch)?
                    .Replace('.', ',');

                if (!string.IsNullOrWhiteSpace(result))
                {
                    var split = result.Split(',');

                    if (split.Length > 1)
                    {
                        var width = int.Parse(split[0]);
                        var height = int.Parse(split[1]);

                        if (split.Length > 2)
                        {
                            size = new Size(height, width);
                        }
                        else
                        {
                            size = new Size(width, height);
                        }

                        return true;
                    }
                }
            }

            size = default;
            return false;
        }
    }
}
