﻿using CommandLine;
using System.IO;
using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Services.Video;
using static System.Console;
using static Uhost.Core.Data.Entities.File;

namespace Uhost.Console.Commands
{
    [Verb("migratevideos", HelpText = "Перемещает файлы, относящиеся к видео в папку видео")]
    public sealed class MigrateVideosCommand : BaseCommand
    {
        protected override void Run()
        {
            var context = Provider.GetDbContextScope<PostgreSqlDbContext>();
            var types = VideoService.VideoFileTypes.Select(e => e.ToString()).ToList();
            var entities = context.Files
                .Where(e => types.Contains(e.Type) && e.DeletedAt == null)
                .ToList();

            context.DetachAllEntities();

            var files = entities.Select(e => new
            {
                e.Id,
                Old = new FileInfo(e.GetPath(Types.Other)),
                New = new FileInfo(e.GetPath())
            })
                .Where(e => e.Old.Exists)
                .ToList();

            if (!files.Any())
            {
                WriteLine("Нет файлов для миграции");
                return;
            }

            foreach (var file in files)
            {
                Write($"{file.Id}\t{file.Old}  ->  {file.New}");

                try
                {
                    Tools.MakePath(file.New.FullName);
                    file.Old.MoveTo(file.New.FullName, true);
                    WriteLine("\tOk!!");
                }
                catch
                {
                    WriteLine("\tFail");
                }
            }
        }
    }
}
