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
                .OutputToFile(Path.Combine(Path.GetTempPath(), "test480.mp4"), true, e => e.ApplyOptimalPreset(mediaInfo, FileTypes.Video480p, TimeSpan.FromSeconds(10)));
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
                .OutputToHls(Path.Combine(Path.GetTempPath(), "test.hls"), e => e.ApplyOptimalPreset(mediaInfo, FileTypes.Video480p));

            await ffargs.ProcessAsynchronously(true);
        }

        [Fact]
        public void ExtensionsToMime()
        {
            //var exts = new[] { "3g2", "3gp2", "3gp", "3gpp", "asf", "asr", "asx", "avi", "dvr", "flv", "ivf", "lsf", "lsx", "m1v", "m2ts", "m4v", "mov", "movie", "mp2", "mp4", "mp4v", "mpa", "mpe", "mpeg", "mpg", "mpv2", "nsc", "ogg", "ogv", "qt", "ts", "tts", "webm", "wm", "wmp", "wmv", "wmx", "wtv", "wvx" };
            var exts = new[] { "jpg", "jfif", "jpeg", "jpe", "gif", "png", "bmp", "rle", "dib" };
            var mimes = exts
                .Select(e => $".{e}".GetContentType())
                .Distinct()
                .OrderBy(e => e)
                .Join(", ");
        }
    }
}
