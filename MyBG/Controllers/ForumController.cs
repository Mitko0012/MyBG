﻿using AspNetCoreGeneratedDocument;
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
            model.ForumPosts = _ctx.Posts.Where(x => !x.IsDeleted).Include(x => x.LikedUser).ToList();
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
            question.User = _manager.GetUserAsync(User).Result;
            if(question == null) 
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
            ForumQuestion? question = _ctx.Posts.Where(x => !x.IsDeleted).Include(x => x.User).Include(x => x.LikedUser).Include(x => x.Comment).ThenInclude(x => x.LikedUser).Include(x => x.Comment).ThenInclude(x => x.User).Include(x => x.Comment).ThenInclude(x => x.Replies).ThenInclude(x => x.LikedUser).FirstOrDefault(x => x.Id == id);
            if (!ModelState.IsValid || question == null)
            {
                return RedirectToAction("Index");
            }
            question.Pfp = _ctx.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(x => x.UserName == question.User.UserName);
            question.Comment = question.Comment.Where(x => !x.IsDeleted).ToList();
            if (commentDisplayCount != null)
            {
                question.CommentCount = (int)commentDisplayCount;
            }
            foreach (var item in question.Comment)
            {
                item.PFP = _ctx.PFPs.FirstOrDefault(x => x.UserName == item.User.UserName);
                item.Replies = item.Replies.Where(x => !x.IsDeleted).ToList();
            }
            return View(question);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LikePost(int id, int replyCount)
        {
            ForumQuestion question = _ctx.Posts.Where(x => !x.IsDeleted).Include(x => x.LikedUser).Include(x => x.Comment).FirstOrDefault(x => x.Id == id);
            IdentityUser currentUser = await _manager.GetUserAsync(User);
            PFPModel model = _ctx.PFPs.FirstOrDefault(x => x.UserName == currentUser.UserName);
            if (!ModelState.IsValid || currentUser == null || model == null || question == null)
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
            ForumQuestion postModel = _ctx.Posts.Where(x => !x.IsDeleted).FirstOrDefault(x => x.Id == id);
            CommentModel commentModel = new CommentModel();
            IdentityUser startUser = await _manager.GetUserAsync(User);
            PFPModel model = _ctx.PFPs.FirstOrDefault(x => x.UserName == startUser.UserName);
            if (!ModelState.IsValid || postModel == null || commentModel == null || startUser == null || model == null)
            {
                return RedirectToAction("PostViewer", new { id = id });
            }
            commentModel.PostId = id;
            commentModel.PageId = 0;
            commentModel.Text = CommentCurrent;
            commentModel.User = startUser;
            commentModel.PFP = model;
            postModel.Comment.Add(commentModel);
            _ctx.Comments.Add(commentModel);
            _ctx.SaveChanges();
            return RedirectToAction("PostViewer", new { id = id });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LikeComment(int id, int replyCount)
        {
            CommentModel comment = _ctx.Comments.Where(x => !x.IsDeleted).Include(x => x.LikedUser).FirstOrDefault(x => x.Id == id);
            IdentityUser user = await _manager.GetUserAsync(User);
            PFPModel model = _ctx.PFPs.FirstOrDefault(x => x.UserName == user.UserName);
            if (!ModelState.IsValid || user == null || comment == null || model == null)
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

        [Authorize]
        public IActionResult PostReply(int commentId, PageModel cmnt) 
        {
            CommentModel comment = _ctx.Comments.Where(x => !x.IsDeleted).FirstOrDefault(x => x.Id == commentId);
            if(comment == null)
            {
                return RedirectToAction("Index");
            }
            CommentModel reply = new CommentModel()
            {
                Text = cmnt.Comment,
                User = _manager.GetUserAsync(User).Result
            };
            comment.Replies.Add(reply);
            _ctx.Comments.Add(reply);
            _ctx.SaveChanges();
            return RedirectToAction();
        }
        [Authorize]
        public IActionResult PostReplyForum(int commentId, ForumQuestion cmnt) 
        {
            CommentModel comment = _ctx.Comments.Where(x => !x.IsDeleted).FirstOrDefault(x => x.Id == commentId);
            if(comment == null)
            {
                return RedirectToAction("Index");
            }
            CommentModel reply = new CommentModel()
            {
                Text = cmnt.CommentCurrent,
                User = _manager.GetUserAsync(User).Result
            };
            comment.Replies.Add(reply);
            _ctx.Comments.Add(reply);
            _ctx.SaveChanges();
            return RedirectToAction();
        }
    }
}
