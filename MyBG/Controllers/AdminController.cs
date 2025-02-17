using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBG.Data;
using MyBG.Models;

namespace MyBG.Controllers
{
    public class Admin : Controller
    {
        ApplicationDbContext _dbContext;
        SignInManager<IdentityUser> _manager;
        public Admin(ApplicationDbContext context, SignInManager<IdentityUser> manager)
        {
            _dbContext = context;
            _manager = manager;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index() 
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Users()
        {
            List<PFPModel> models = new List<PFPModel>();
            models = _dbContext.PFPs.Where(p => !p.IsDeleted).ToList();
            Users users = new Users();
            users.AllUsers = new List<IdentityUser>();
            foreach (var model in models)
            {
                users.AllUsers.Add(_dbContext.Users.FirstOrDefault(x => x.UserName == model.UserName));
            }
            users.AllUsers.OrderBy((x) => _manager.UserManager.GetRolesAsync(x).Result.Contains("Admin") ? 0 : 1).ToList();
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditSubmissions()
        {
            ViewEditsModel cont = new ViewEditsModel();
            cont.Edits = _dbContext.Edits.Where((x) => !x.Approved && !x.IsDeleted).Include(x => x.PageToEdit).Include(x => x.UserCreated).ToList();
            return View(cont);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditSubmissionAdmin(int id)
        {
            EditModel? submission = _dbContext.Edits.Where(x => !x.Approved && !x.IsDeleted).Include(x => x.PageToEdit).Include(x => x.UserCreated).FirstOrDefault(x => x.ID == id);
            if (!ModelState.IsValid || submission == null)
            {
                return RedirectToAction("Index");
            }
            return View(submission);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditSubmissionApprove(int id)
        {
            ApproveModel model = new ApproveModel();
            model.EditModel = _dbContext.Edits.Where(x => !x.IsDeleted).Include(x => x.PageToEdit).FirstOrDefault(x => x.ID == id);
            if (model.EditModel == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult ConfirmApprove(int id, ApproveModel approve)
        {
            approve.EditModel = _dbContext.Edits.Where(x => !x.IsDeleted).Include(x => x.PageToEdit).Include(x => x.UserCreated).FirstOrDefault(x => x.ID == id);
            if (approve == null || approve.EditModel == null)
            {
                return RedirectToAction("Index");
            }
            PFPModel? user = _dbContext.PFPs.Include(x => x.Contributions).FirstOrDefault(x => x.UserName == _manager.UserManager.GetUserAsync(User).Result.UserName);
            approve.EditModel.Approved = true;
            approve.EditModel.PageToEdit.TextBody = approve.EditModel.NewText;
            approve.EditModel.PageIndex = approve.EditModel.PageToEdit.Edits.Where(x => x.Approved).ToList().Count;
            if (approve.EditModel.CreatePage)
            {
                approve.EditModel.PageToEdit.Approved = true;
            }
            InboxMessage message = new InboxMessage()
            {
                Message = approve.MessageForApproved
            };
            approve.EditModel.UserCreated.Inbox.Add(message);
            _dbContext.Messages.Add(message);
           _dbContext.SaveChanges();
            foreach (EditModel edit in _dbContext.Edits.Where(x => x.Approved == false && x.PageToEdit.Title == approve.EditModel.PageToEdit.Title))
            {
                edit.IsDeleted = true;
                InboxMessage message2 = new InboxMessage()
                {
                    Message = approve.MessageForDeclined
                };
                edit.UserCreated.Inbox.Add(message2);
                _dbContext.Messages.Add(message2);
            }
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EditSubmissionDecline(int id)
        {
            ApproveModel model = new ApproveModel();
            model.EditModel = _dbContext.Edits.Where(x => !x.IsDeleted).Include(x => x.PageToEdit).FirstOrDefault(x => x.ID == id);
            if (model.EditModel == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditSubmissionDeclineConfirm(int id, ApproveModel decline)
        {
            EditModel? model = _dbContext.Edits.Where(x => !x.Approved && !x.IsDeleted).Include(x => x.PageToEdit).FirstOrDefault(x => x.ID == id);
            decline.EditModel = model;
            if (!ModelState.IsValid || model == null || decline == null)
            {
                return RedirectToAction("Index");
            }
            model.IsDeleted = true;
            if (model.CreatePage)
            {
                model.PageToEdit.IsDeleted = true;
            }
            InboxMessage message = new InboxMessage()
            {
                Message = decline.MessageForApproved
            };
            model.UserCreated.Inbox.Add(message);
            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteForumPost(int id)
        { 
            DeclinePostModel model = new DeclinePostModel();
            model.Post = _dbContext.Posts.Where(x => !x.IsDeleted).First(x => x.Id == id);
            if(model.Post == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteForumPostConfirm(int id, DeclinePostModel approve)
        {
            ForumQuestion? post = _dbContext.Posts.Where(x => !x.IsDeleted).Include(x => x.Comment).Include(x => x.User).FirstOrDefault(x => x.Id == id);
            PFPModel model = _dbContext.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(x => x.UserName == post.User.UserName);
            if (post == null || model == null)
            {
                return RedirectToAction("Index", "Page");
            }
            post.IsDeleted = true;
            foreach (CommentModel comment in post.Comment)
            {
                comment.IsDeleted = true;
            }
            InboxMessage message = new InboxMessage();
            message.Message = approve.DeclineMessage;
            model.Inbox.Add(message);
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Forum");
        }


        [Authorize(Roles = "Admin")]
        public IActionResult DeleteComment(int id)
        {
            CommentDeleteMessage message = new CommentDeleteMessage()
            {
                Id = id
            };
            return View(message);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteCommentConfirm(int id, CommentDeleteMessage messagePassed)
        {
            CommentModel? comment = _dbContext.Comments.Where(x => !x.IsDeleted).Include(x => x.User).FirstOrDefault(x => x.Id == id);
            comment.PFP = _dbContext.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(x => x.UserName == comment.User.UserName);
            if (comment == null || messagePassed == null || comment.PFP == null)
            {
                return RedirectToAction("Index", "Page");
            }
            comment.IsDeleted = true;
            InboxMessage message1 = new InboxMessage()
            {
                Message = messagePassed.Message
            };
            _dbContext.Messages.Add(message1);
            comment.PFP.Inbox.Add(message1);
            _dbContext.SaveChanges();
            return RedirectToAction("PageViewer", "Page", new { id = comment.PageId });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteCommentFromPost(int id)
        {
            CommentPostDeleteMessage message = new CommentPostDeleteMessage()
            {
                Id = id,
            };
            return View(message);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteCommentFromPostConfirm(int id, CommentPostDeleteMessage messagePassed)
        {
            CommentModel? comment = _dbContext.Comments.Include(x => x.User).FirstOrDefault(x => x.Id == id);
            comment.PFP = _dbContext.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(x => x.UserName == comment.User.UserName);
            if (comment == null || comment.User == null || comment.PFP == null)
            {
                return RedirectToAction("Index", "Page");
            }
            InboxMessage message1 = new InboxMessage()
            {
                Message = messagePassed.Message
            };
            _dbContext.Messages.Add(message1);
            comment.PFP.Inbox.Add(message1);
            comment.IsDeleted = true;
            _dbContext.SaveChanges();
            return RedirectToAction("PostViewer", "Forum", new { id = comment.PostId });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(string userName)
        {
            PFPModel? model = _dbContext.PFPs.FirstOrDefault(p => p.UserName == userName);
            IdentityUser? user = _dbContext.Users.FirstOrDefault(u => u.UserName == userName);
            if(user == null || model == null || _manager.UserManager.GetRolesAsync(user).Result.Contains("Admin"))
            {
                return RedirectToAction("Index", "Page");
            }
            model.IsDeleted = true;
            _dbContext.SaveChanges();
            return RedirectToAction("Users");
        }
    }
}
