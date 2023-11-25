using System;
using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models.File;
using Uhost.Core.Models.User;
using Uhost.Core.Models.Video;
using Uhost.Core.Models.VideoReaction;
using Uhost.Core.Repositories;
using Uhost.Core.Services.File;
using static Uhost.Core.Data.Entities.File;
using static Uhost.Core.Data.Entities.Right;
using Entity = Uhost.Core.Data.Entities.VideoReaction;
using UserEntity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Services.VideoReaction
{
    public sealed class VideoReactionService : BaseService, IVideoReactionService
    {
        private static readonly string _reactionDetailSql = @$"EXPLAIN ANALYZE WITH vr AS (
    SELECT
        ""{nameof(Entity.UserId)}"",
        ""{nameof(Entity.Value)}"",
        ROW_NUMBER() OVER(PARTITION BY ""{nameof(Entity.Value)}"" ORDER BY ""{nameof(Entity.CreatedAt)}"" DESC) AS num

    FROM ""{Tools.GetEntityTableName<Entity>()}""
    WHERE 
        ""{nameof(Entity.DeletedAt)}"" IS NULL
        AND ""{nameof(Entity.VideoId)}"" = @{nameof(Entity.VideoId)}
)
SELECT * FROM vr
WHERE num < 11";
        private readonly VideoReactionRepository _repo;
        private readonly VideoRepository _videoRepo;
        private readonly UserRepository _userRepo;
        private readonly IFileService _fileService;

        public VideoReactionService(PostgreSqlDbContext dbContext, IServiceProvider provider, IFileService fileService) : base(dbContext, provider)
        {
            _repo = new VideoReactionRepository(_dbContext);
            _videoRepo = new VideoRepository(_dbContext);
            _userRepo = new UserRepository(_dbContext);
            _fileService = fileService;
        }

        public VideoReactionSummaryViewModel GetOne(string videoToken)
        {
            var videoId = _videoRepo.GetId(videoToken);

            if (videoId == 0)
            {
                return null;
            }

            var model = new VideoReactionSummaryViewModel
            {
                Reactions = _repo.GetReactionsByOneVideo(videoId)
            };

            var userDetails = _repo
                .FromSqlRaw<int, string>(_reactionDetailSql, new Npgsql.NpgsqlParameter(nameof(Entity.VideoId), videoId))
                .Select(e => new { UserId = e.Item1, Value = e.Item2.ParseEnum<Entity.Reactions>() })
                .Where(e => e.Value is Entity.Reactions)
                .Select(e => new { e.UserId, Value = (Entity.Reactions)e.Value })
                .ToList();

            if (userDetails.Any())
            {
                var userQuery = new UserQueryModel
                {
                    Ids = userDetails.Select(e => e.UserId)
                };
                var users = _userRepo
                    .GetAll<UserCommentViewModel>(userQuery)
                    .ToList();
                var avatars = _fileService
                    .GetByDynEntity<FileShortViewModel>(userQuery.Ids, typeof(UserEntity), Types.UserAvatar)
                    .ToList();

                foreach (var user in users)
                {
                    user.AvatarUrl = avatars.FirstOrDefault(e => e.DynId == user.Id)?.Url;
                }
            }

            return model;
        }

        public Entity Set(string videoToken, Entity.Reactions reaction)
        {
            var model = new VideoReactionCreateModel
            {
                Value = reaction.ToString(),
                VideoId = _videoRepo.GetId(videoToken)
            };

            if (TryGetUserId(out var userId))
            {
                model.UserId = userId;
            }

            return _repo.AddOrUpdate(model, e => e.VideoId == model.VideoId && e.UserId == userId && e.DeletedAt == null);
        }

        public bool Remove(string videoToken)
        {
            if (TryGetUserId(out var userId))
            {
                var videoId = _videoRepo.GetId(videoToken);
                var affected = _repo.Perform(e => e.DeletedAt = DateTime.Now, e => e.UserId == userId && e.VideoId == videoId && e.DeletedAt == null);

                return affected > 0;
            }

            return false;
        }

        public bool CheckUserRestrictions(string videoToken, out Rights missing)
        {
            missing = default;

            if (!TryGetUserId(out var userId))
            {
                return false;
            }

            var video = _videoRepo
                .GetAll<VideoShortViewModel>(new VideoQueryModel { Token = videoToken })
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

        public bool AreReactionsAllowed(string videoToken)
        {
            var query = new VideoQueryModel
            {
                Token = videoToken,
                AllowReactions = true,
                ShowPrivate = true,
                ShowHidden = true
            };

            return _videoRepo.PrepareQuery(query).Any();
        }
    }
}
