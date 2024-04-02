using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Uhost.Core.Models.File;
using Uhost.Core.Models.Playlist;
using Uhost.Core.Models.Video;
using Uhost.Core.Repositories;
using Uhost.Core.Services.File;
using static Uhost.Core.Data.Entities.File;
using static Uhost.Core.Data.Entities.Right;
using Entity = Uhost.Core.Data.Entities.Playlist;
using QueryModel = Uhost.Core.Models.Playlist.PlaylistQueryModel;
using UserEntity = Uhost.Core.Data.Entities.User;
using VideoEntity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Services.Playlist
{
    public class PlaylistService : BaseService, IPlaylistService
    {
        private readonly PlaylistRepository _repo;
        private readonly IFileService _files;

        public PlaylistService(
            IDbContextFactory<PostgreSqlDbContext> factory,
            IServiceProvider provider,
            IFileService files) : base(factory, provider)
        {
            _repo = new PlaylistRepository(_dbContext);
            _files = files;
        }

        private static bool IsVideoAvailableForUser(VideoShortViewModel model, int userId, IEnumerable<Rights> userRights)
        {
            if (model.UserId == userId)
            {
                return true;
            }
            if (userRights.Contains(Rights.VideoGetAll))
            {
                return true;
            }

            return !model.IsPrivate;
        }

        public PagerResultModel<PlaylistViewModel> GetAllPaged(QueryModel query)
        {
            var pager = _repo.GetAll<PlaylistViewModel>(query)
                .CreatePager(query);

            if (pager.Any())
            {
                TryGetUserId(out var userId);
                TryGetUserRights(out var rights);

                var avatars = _files
                    .GetByDynEntity<FileShortViewModel>(pager.SelectMany(e => e.Videos).Select(e => e.UserId).Concat(pager.Select(e => e.UserId)), typeof(UserEntity), FileTypes.UserAvatar)
                    .ToList();

                foreach (var model in pager.Where(e => e.Videos != null))
                {
                    model.Videos = model.Videos.Where(e => IsVideoAvailableForUser(e, userId, rights)).ToList();

                    if (model.User != null)
                    {
                        model.User.AvatarUrl = avatars.FirstOrDefault(e => e.DynId == model.UserId)?.Url;
                    }
                }

                var thumbs = _files
                    .GetByDynEntity<FileShortViewModel>(pager.SelectMany(e => e.Videos).Select(e => e.Id), typeof(VideoEntity), FileTypes.VideoThumbnail)
                    .ToList();

                foreach (var model in pager.SelectMany(e => e.Videos))
                {
                    model.ThumbnailUrl = thumbs.FirstOrDefault(e => e.DynId == model.Id)?.Url;

                    if (model.User != null)
                    {
                        model.User.AvatarUrl = avatars.FirstOrDefault(e => e.DynId == model.UserId)?.Url;
                    }
                }
            }

            return pager.Paginate();
        }

        public PlaylistViewModel GetOne(int id)
        {
            var model = _repo.GetAll<PlaylistViewModel>(new QueryModel { Id = id }).FirstOrDefault();

            if (model != null)
            {
                TryGetUserId(out var userId);
                TryGetUserRights(out var rights);

                var thumbs = _files
                    .GetByDynEntity<FileShortViewModel>(model.Videos.Select(e => e.Id), typeof(VideoEntity), FileTypes.VideoThumbnail)
                    .ToList();

                var avatars = _files
                    .GetByDynEntity<FileShortViewModel>(model.Videos.Select(e => e.UserId).Concat(model.UserId.AsSingleElementEnumerable()), typeof(UserEntity), FileTypes.UserAvatar)
                    .ToList();

                model.Videos = model.Videos.Where(e => IsVideoAvailableForUser(e, userId, rights)).ToList();

                if (model.User != null)
                {
                    model.User.AvatarUrl = avatars.FirstOrDefault(e => e.DynId == model.UserId)?.Url;
                }

                foreach (var video in model.Videos)
                {
                    video.ThumbnailUrl = thumbs.FirstOrDefault(e => e.DynId == video.Id)?.Url;

                    if (video.User != null)
                    {
                        video.User.AvatarUrl = avatars.FirstOrDefault(e => e.DynId == video.UserId)?.Url;
                    }
                }
            }

            return model;
        }

        public Entity Create(PlaylistCreateModel model)
        {
            if (TryGetUserId(out var userId))
            {
                model.UserId = userId;
            }

            var entity = _repo.Add(model);

            return entity;
        }

        public void Update(int id, PlaylistUpdateModel model)
        {
            _repo.Update(id, model);
        }

        public void Delete(int id)
        {
            _repo.SoftDelete(id);
        }
    }
}
