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
        SignInManager<IdentityUser> _signInManager;
        public ForumController(ApplicationDbContext ctx, UserManager<IdentityUser> manager, SignInManager<IdentityUser> signInManager) 
        {
            _ctx = ctx;
            _manager = manager;
            _signInManager = signInManager;
        }

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
        public ActionResult PostViewer(int id, string? scroll, string? replyString)
        {
            ForumQuestion? question = _ctx.Posts.Where(x => !x.IsDeleted).Include(x => x.User).Include(x => x.LikedUser).Include(x => x.Comment).ThenInclude(x => x.LikedUser).Include(x => x.Comment).ThenInclude(x => x.User).Include(x => x.Comment).ThenInclude(x => x.Replies).ThenInclude(x => x.LikedUser).Include(x => x.Comment).ThenInclude(x => x.Replies).ThenInclude(x => x.User).FirstOrDefault(x => x.Id == id);
            PFPModel? userModel = new PFPModel();
            if(_signInManager.IsSignedIn(User))
            {
                userModel = _ctx.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(x => x.UserName == _manager.GetUserAsync(User).Result.UserName);
            }
            if (question == null)
            {
                return RedirectToAction("Index");
            }
            if(_signInManager.IsSignedIn(User))
            {
                if (_signInManager.IsSignedIn(User) && question.LikedUser.FirstOrDefault(x => x.UserName == userModel.UserName) != null)
                {
                    question.LikedByUser = true;
                }
                else
                {
                    question.LikedByUser = false;
                }
            }   
            question.Scroll = scroll ?? "0";
            question.ReplyString = replyString ?? "d";
            question.Pfp = _ctx.PFPs.FirstOrDefault(x => x.UserName == question.User.UserName);
            question.Comment = question.Comment.Where(x => !x.IsDeleted).ToList();
            foreach (var item in question.Comment)
            {
                item.PFP = _ctx.PFPs.FirstOrDefault(x => x.UserName == item.User.UserName);
                item.Replies = item.Replies.Where(x => !x.IsDeleted).ToList();
                if(_signInManager.IsSignedIn(User))
                {
                    if (item.LikedUser.FirstOrDefault(x => x.UserName == userModel.UserName) != null)
                    {
                        item.LikedByUser = true;
                    }
                    else
                    {
                        item.LikedByUser = false;
                    } 
                }
                foreach(var reply in item.Replies)
                {
                    reply.PFP = _ctx.PFPs.FirstOrDefault(x => x.UserName == reply.User.UserName);
                    if(_signInManager.IsSignedIn(User))
                    {
                        if (_signInManager.IsSignedIn(User) && reply.LikedUser.FirstOrDefault(x => x.UserName == userModel.UserName) != null)
                        {
                            reply.LikedByUser = true;
                        }
                        else
                        {
                            reply.LikedByUser = false;
                        }
                    }
                }
            }
            return View(question);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LikePost(int id)
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
            return RedirectToAction("PostViewer", new { id = id});
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CommentPost(string CommentCurrent, int id, string scroll, string replyString)
        {
            if(CommentCurrent == null)
            {
                CommentCurrent = "";
            }
            ForumQuestion postModel = _ctx.Posts.Where(x => !x.IsDeleted).FirstOrDefault(x => x.Id == id);
            CommentModel commentModel = new CommentModel();
            IdentityUser startUser = await _manager.GetUserAsync(User);
            PFPModel model = _ctx.PFPs.FirstOrDefault(x => x.UserName == startUser.UserName);
            if (postModel == null || commentModel == null || startUser == null || model == null)
            {
                return RedirectToAction("PostViewer", new { id = id, scroll = scroll, replyString = replyString });
            }
            commentModel.PostId = id;
            commentModel.PageId = 0;
            commentModel.Text = CommentCurrent;
            commentModel.User = startUser;
            commentModel.PFP = model;
            postModel.Comment.Add(commentModel);
            _ctx.Comments.Add(commentModel);
            _ctx.SaveChanges();
            return RedirectToAction("PostViewer", new { id = id, scroll = scroll, replyString = replyString });
        }
        [Authorize]
        public async Task<IActionResult> LikeComment(int id, int forumId, string scroll, string replyString)
        {
            CommentModel comment = _ctx.Comments.Where(x => !x.IsDeleted).Include(x => x.LikedUser).FirstOrDefault(x => x.Id == id);
            IdentityUser user = await _manager.GetUserAsync(User);
            PFPModel model = _ctx.PFPs.FirstOrDefault(x => x.UserName == user.UserName);
            if (user == null || comment == null || model == null)
            {
                return RedirectToAction("PostViewer", new { id = comment.PostId });
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
            return RedirectToAction("PostViewer", new { id = forumId, scroll = scroll, replyString = replyString });
        }

        [Authorize]
        public IActionResult PostReply(int commentId, PageModel cmnt, string? scroll, string replyString) 
        {
            CommentModel comment = _ctx.Comments.Where(x => !x.IsDeleted).FirstOrDefault(x => x.Id == commentId);
            if(cmnt.Comment == null)
            {
                cmnt.Comment = "";
            }
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
            if(cmnt.IsCulture)
            {
                return RedirectToAction("CulturePage", "Page", new {id = cmnt.Id, replyCount = cmnt.CommenntsToDisplay, scroll = scroll, replyString = replyString});
            }
            else
            {
                return RedirectToAction("PageViewer", "Page", new {id = cmnt.Id, replyCount = cmnt.CommenntsToDisplay, scroll = scroll, replyString = replyString});
            }
        }
        [Authorize]
        public IActionResult PostReplyForum(int commentId, ForumQuestion cmnt, string scroll, string replyString) 
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
            return RedirectToAction("PostViewer", new {id = cmnt.Id, scroll = scroll, replyString = replyString});
        }
    }
}
