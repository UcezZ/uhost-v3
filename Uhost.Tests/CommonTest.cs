using FFMpegCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uhost.Core.Extensions;
using Xunit;
using static Uhost.Core.Data.Entities.File;

namespace Uhost.Tests
{
    public class CommonTest
    {
        private const string _sourceDir = @"X:\Common\Media";
        private readonly Random _r = new Random();

        [Fact]
        public async Task FF()
        {
            var dir = new DirectoryInfo(_sourceDir);

            if (!dir.Exists)
            {
                return;
            }

            var file = dir.GetFiles()
                .Where(e => e.Extension.IsMatchesRegex(@"mp\S|mkv"))
                .OrderBy(e => _r.NextDouble())
                .FirstOrDefault();

            if (file == null)
            {
                return;
            }

            var mediaInfo = await FFProbe.AnalyseAsync(file.FullName);
            var ffargs = FFMpegArguments
                .FromFileInput(file.FullName)
                .OutputToFile(Path.Combine(Path.GetTempPath(), "test480.mp4"), true, e => e.ApplyPreset(mediaInfo, Types.Video480p, TimeSpan.FromSeconds(10)));
            await ffargs.ProcessAsynchronously(true);
        }
    }
}
