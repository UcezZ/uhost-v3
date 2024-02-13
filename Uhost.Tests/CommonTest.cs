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

        private static FileInfo GetAnyMediaFile()
        {
            var dir = new DirectoryInfo(_sourceDir);
            var file = dir.GetFiles()
                .Where(e => e.Extension.IsMatchesRegex(@"mp(\S{1,2})|mkv"))
                .RandomOrDefault();

            return file;
        }

        [Fact]
        public async Task ConversionTest()
        {
            var file = GetAnyMediaFile();

            Assert.NotNull(file);

            var mediaInfo = await FFProbe.AnalyseAsync(file.FullName);
            var ffargs = FFMpegArguments
                .FromFileInput(file.FullName)
                .OutputToFile(Path.Combine(Path.GetTempPath(), "test480.mp4"), true, e => e.ApplyOptimalPreset(mediaInfo, Types.Video480p, TimeSpan.FromSeconds(10)));
            await ffargs.ProcessAsynchronously(true);
        }

        [Fact]
        public async Task HlsArguments()
        {
            var file = GetAnyMediaFile();

            Assert.NotNull(file);

            var mediaInfo = await FFProbe.AnalyseAsync(file.FullName);
            var ffargs = FFMpegArguments
                .FromFileInput(file.FullName)
                .OutputToHls(Path.Combine(Path.GetTempPath(), "test.hls"), e => e.ApplyOptimalPreset(mediaInfo, Types.Video480p));

            await ffargs.ProcessAsynchronously(true);
        }
    }
}
