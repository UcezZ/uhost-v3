using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uhost.Core.Models.Comment;
using Uhost.Core.Services.Comment;
using Uhost.Web.Common;
using Uhost.Web.Extensions;
using QueryModel = Uhost.Core.Models.Comment.CommentQueryModel;

namespace Uhost.Web.Controllers
{
    [Route("api/v2/comments")]
    public class CommentController : Controller
    {
        private readonly ICommentService _service;

        public CommentController(ICommentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получение комментариев
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public IActionResult GetAllPaged(QueryModel query)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            return ResponseHelper.Success(_service.GetAllPaged(query));
        }

        [HttpPost]
        public IActionResult Create(CommentCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            var entity = _service.Add(model);

            throw null;
            //return ResponseHelper.Success(_service.GetOne(entity.Id));
        }
    }
}
