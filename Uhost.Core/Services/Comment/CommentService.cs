﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models.Comment;
using Uhost.Core.Models.File;
using Uhost.Core.Models.Video;
using Uhost.Core.Repositories;
using Uhost.Core.Services.File;
using Uhost.Core.Services.Log;
using static Uhost.Core.Data.Entities.File;
using static Uhost.Core.Data.Entities.Log;
using static Uhost.Core.Data.Entities.Right;
using Entity = Uhost.Core.Data.Entities.Comment;
using QueryModel = Uhost.Core.Models.Comment.CommentQueryModel;
using UserEntity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Services.Comment
{
    public class CommentService : BaseService, ICommentService
    {
        private readonly CommentRepository _repo;
        private readonly VideoRepository _videoRepo;
        private readonly IFileService _fileService;
        private readonly ILogService _log;

        public CommentService(
            IDbContextFactory<PostgreSqlDbContext> factory,
            IServiceProvider provider,
            IFileService fileService,
            ILogService log) : base(factory, provider)
        {
            _repo = new CommentRepository(_dbContext);
            _videoRepo = new VideoRepository(_dbContext);
            _fileService = fileService;
            _log = log;
        }

        public object GetAllPaged(QueryModel query)
        {
            var pager = _repo
                .GetAll<CommentViewModel>(query)
                .CreatePager(query);

            if (pager.Any())
            {
                var files = _fileService
                    .GetByDynEntity<FileShortViewModel>(pager.Select(e => e.UserId), typeof(UserEntity), FileTypes.UserAvatar)
                    .ToList();

                foreach (var user in pager.Select(e => e.User))
                {
                    user.AvatarUrl = files.FirstOrDefault(e => e.DynId == user.Id)?.Url;
                }
            }

            return pager.Paginate();
        }

        public CommentViewModel GetOne(int id)
        {
            var model = _repo
                .GetAll<CommentViewModel>(new QueryModel { Id = id })
                .FirstOrDefault();

            if (model?.User != null)
            {
                var userAvatar = _fileService
                    .GetByDynEntity<FileShortViewModel>(model.UserId, typeof(UserEntity), FileTypes.UserAvatar)
                    .FirstOrDefault();
                model.User.AvatarUrl = userAvatar?.Url;
            }

            return model;
        }

        public Entity Add(string videoToken, string text)
        {
            var model = new CommentCreateModel
            {
                Text = text,
                VideoId = _videoRepo.GetId(videoToken)
            };

            if (TryGetUserId(out var userId))
            {
                model.UserId = userId;
            }

            _log.Add(Events.CommentPosted, model);

            return _repo.Add(model);
        }

        public bool Delete(string videoToken, int id)
        {
            var affected = _repo.Perform(e => e.DeletedAt = DateTime.Now, e => e.Id == id && e.DeletedAt == null && e.Video.Token == videoToken);

            _log.Add(Events.CommentDeleted, new
            {
                videoToken,
                id,
                affected
            });

            return affected > 0;
        }

        public bool CheckUserRestrictions(string videoToken, out Rights missing)
        {
            missing = default;

            if (!TryGetUserId(out var userId))
            {
                return false;
            }

            var video = _videoRepo
                .GetAll<VideoShortViewModel>(new VideoQueryModel
                {
                    ShowHidden = true,
                    ShowPrivateForUserId = userId,
                    Token = videoToken
                })
                .FirstOrDefault();

            if (video == null || !TryGetUserRights(out var rights))
            {
                return false;
            }

            if (video.UserId != userId && video.IsPrivate && !rights.Contains(Rights.VideoGetAll))
            {
                missing = Rights.VideoGetAll;

                return false;
            }

            return true;
        }

        public bool AreCommentsAllowed(string videoToken)
        {
            var query = new VideoQueryModel
            {
                Token = videoToken,
                AllowComments = true,
                ShowPrivate = true,
                ShowHidden = true
            };

            return _videoRepo.PrepareQuery(query).Any();
        }
    }
}
