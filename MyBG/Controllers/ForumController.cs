using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBG.Data;
using MyBG.Models;

namespace MyBG.Controllers
{
    public class ForumController : Controller
    {
        ApplicationDbContext _ctx;
        UserManager<IdentityUser> _manager;
        public ForumController(ApplicationDbContext ctx, UserManager<IdentityUser> manager) 
        {
            _ctx = ctx;
            _manager = manager;
        }

        [Authorize]
        public IActionResult Index(string? sortingType, string? searchString)
        {
            ForumContainer model = new ForumContainer();
            model.ForumPosts = _ctx.Posts.Include(x => x.LikedUser).ToList();
            if (sortingType == null)
                sortingType = "NewestFirst";
            if(searchString == null)
                searchString = string.Empty;
            if(sortingType != "NewestFirst" && sortingType != "MostLiked" && sortingType != "Search")
            {
                return RedirectToAction("Index");
            }
            switch(sortingType)
            {
                case "NewestFirst":
                    model.ForumPosts.Reverse();
                    break;
                case "MostLiked":
                    model.ForumPosts.Reverse();
                    model.ForumPosts = model.ForumPosts.OrderBy(x => -x.LikesReturn).ToList();
                    break;
                case "Search":
                    model.ForumPosts.Reverse();
                    model.ForumPosts = model.ForumPosts.Where(x => x.Title.Contains(searchString)).ToList();
                    break;
            }
            return View(model);
        }
        [Authorize]
        public IActionResult Add()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult PostForum(ForumQuestion question)
        {
            question.Date = DateTime.Today;
            if(!ModelState.IsValid) 
            {
                return RedirectToAction("Add");
            }
            _ctx.Posts.Add(question);
            _ctx.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
        public ActionResult PostViewer(int id, int? commentDisplayCount)
        {
            ForumQuestion question = _ctx.Posts.Include(x => x.LikedUser).Include(x => x.Comment).ThenInclude(x => x.LikedUser).Include(x => x.Comment).ThenInclude(x => x.User).FirstOrDefault(x => x.Id == id);
            if (!ModelState.IsValid || question == null)
            {
                return RedirectToAction("Index");
            }
            if (commentDisplayCount != null)
            {
                question.CommentCount = (int)commentDisplayCount;
            }
            foreach (var item in question.Comment)
            {
                item.PFP = _ctx.PFPs.FirstOrDefault(x => x.UserName == item.User.UserName);
            }
            return View(question);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LikePost(int id, int replyCount)
        {
            ForumQuestion question = _ctx.Posts.Include(x => x.LikedUser).Include(x => x.Comment).FirstOrDefault(x => x.Id == id);
            IdentityUser currentUser = await _manager.GetUserAsync(User);
            PFPModel model = _ctx.PFPs.FirstOrDefault(x => x.UserName == currentUser.UserName);
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            if(!question.LikedUser.Contains(model))
            {
                question.LikedUser.Add(model);
            }
            else
            {
                question.LikedUser.Remove(model);
            }
            _ctx.SaveChanges();
            return RedirectToAction("PostViewer", new { id = id , replyCount = replyCount});
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CommentPost(string CommentCurrent, int id)
        {
            ForumQuestion postModel = _ctx.Posts.Find(id);
            CommentModel commentModel = new CommentModel();
            IdentityUser startUser = await _manager.GetUserAsync(User);
            PFPModel model = _ctx.PFPs.FirstOrDefault(x => x.UserName == startUser.UserName);
            commentModel.PostId = id;
            commentModel.PageId = 0;
            commentModel.Text = CommentCurrent;
            commentModel.User = startUser;
            commentModel.PFP = model;
            if (!ModelState.IsValid)
            {
                return RedirectToAction("PostViewer", new {id = id});
            }
            postModel.Comment.Add(commentModel);
            _ctx.Comments.Add(commentModel);
            _ctx.SaveChanges();
            return RedirectToAction("PostViewer", new { id = id });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LikeComment(int id, int replyCount)
        {
            CommentModel comment = _ctx.Comments.Include(x => x.LikedUser).FirstOrDefault(x => x.Id == id);
            IdentityUser user = await _manager.GetUserAsync(User);
            PFPModel model = _ctx.PFPs.FirstOrDefault(x => x.UserName == user.UserName);
            if (!ModelState.IsValid)
            {
                return RedirectToAction("PageViewer", new { id = comment.PostId });
            }
            if (comment.LikedUser.Contains(model))
            {
                comment.LikedUser.Remove(model);
            }
            else
            {
                comment.LikedUser.Add(model);
            }
            _ctx.SaveChanges();
            return RedirectToAction("PostViewer", new { id = comment.PostId, commentDisplayCount = replyCount });
        }
    }
}
