using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models.File;
using Uhost.Core.Repositories;
using Uhost.Core.Services.File;
using Uhost.Core.Services.Video;
using static System.Console;

namespace Uhost.Console.Commands
{
    [Verb("removedeletedvideofiles", HelpText = "Удаляет с диска файлы видео, которые были удалены")]
    public sealed class RemoveDeletedVideoFilesCommand : BaseCommand
    {
        protected override void Run()
        {
            var context = Provider.GetDbContextScope<PostgreSqlDbContext>();

            var repo = new FileRepository(context);
            var files = repo.GetAll<FileShortViewModel>(new FileQueryModel
            {
                Types = VideoService.VideoFileTypes.Select(e => e.ToString()),
                IncludeDeleted = true
            })
                .ToList();

            var deletedVideos = context.Videos.Where(e => e.DeletedAt != null).Select(e => e.Id).ToList();

            var filesToDelete = files
                .Where(e => (e.IsDeleted || deletedVideos.Any(id => id == e.DynId)) && e.Exists)
                .ToList();

            if (!filesToDelete.Any())
            {
                WriteLine("Нет файлов для удаления");
                return;
            }

            WriteLine($"К удалению {filesToDelete.Count} файлов:");

            var service = Provider.GetRequiredService<IFileService>();

            foreach (var file in filesToDelete)
            {
                WriteLine($"  #{file.Id}\t{file.Size.ToHumanSize()}\t{file.Path}");
                service.Delete(file.Id, true);
            }
        }
    }
}
