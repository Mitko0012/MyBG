using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography.Xml;
using Humanizer;
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
        [Authorize(Roles = "Manager,Admin")]
        public IActionResult EditSubmissions()
        {
            ViewEditsModel cont = new ViewEditsModel();
            cont.Edits = _dbContext.Edits.Where((x) => !x.Approved && !x.IsDeleted).Include(x => x.PageToEdit).Where(x => !x.PageToEdit.IsCulture).Include(x => x.UserCreated).ToList();
            return View(cont);
        }

        [Authorize(Roles = "Manager,Admin")]
        public IActionResult EditSubmissionsCulture()
        {
            ViewEditsModel cont = new ViewEditsModel();
            cont.Edits = _dbContext.Edits.Where((x) => !x.Approved && !x.IsDeleted).Include(x => x.PageToEdit).Where(x => x.PageToEdit.IsCulture).Include(x => x.UserCreated).ToList();
            return View(cont);
        }

        [Authorize(Roles = "Manager,Admin")]
        public IActionResult EditSubmissionAdmin(int id)
        {
            EditModel? submission = _dbContext.Edits.Where(x => !x.Approved && !x.IsDeleted).Include(x => x.PageToEdit).Include(x => x.UserCreated).Include(x => x.PageToEdit).ThenInclude(x => x.TransportWays).FirstOrDefault(x => x.ID == id);
            if (!ModelState.IsValid || submission == null)
            {
                return RedirectToAction("Index", "Page");
            }
            return View(submission);
        }

        [Authorize(Roles = "Manager,Admin")]
        public IActionResult EditSubmissionApprove(int id)
        {
            ApproveModel model = new ApproveModel();
            model.EditModel = _dbContext.Edits.Where(x => !x.IsDeleted).Include(x => x.PageToEdit).Include(x => x.UserCreated).FirstOrDefault(x => x.ID == id);
            if (model.EditModel == null)
            {
                return RedirectToAction("Index", "Page");
            }
            model.MessageForApproved = $"Dear {model.EditModel.UserCreated.UserName}, we're delighted to announce that your submission for {(model.EditModel.CreatePage? "creation" : "edit")} of page {model.EditModel.PageToEdit.Title} has been approved! You can now see {(model.EditModel.CreatePage? "the page you created": "the updated contents of the page")} on the destinations area!";
            model.MessageForDeclined = $"Dear user of My BG, we have reviewed your submission for {(model.EditModel.CreatePage? "creation" : "edit")} of page {model.EditModel.PageToEdit.Title}. Unfortunately, the administrators have decided that another submission for the same page seems more appropriate for our platform. Our team thanks you a lot for your decision to contribute to the platform.";
            return View(model);
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public IActionResult ConfirmApprove(int id, ApproveModel approve)
        {
            approve.EditModel = _dbContext.Edits.Where(x => !x.IsDeleted).Include(x => x.PageToEdit).ThenInclude(x => x.Edits).Include(x => x.UserCreated).FirstOrDefault(x => x.ID == id);
            if (approve == null || approve.EditModel == null)
            {
                return RedirectToAction("Index", "Page");
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
                Message = approve.MessageForApproved,
                Title = "Your submission has been approved!"
            };
            approve.EditModel.UserCreated.Inbox.Add(message);
            _dbContext.Messages.Add(message);
           _dbContext.SaveChanges();
            foreach (EditModel edit in _dbContext.Edits.Include(x => x.UserCreated).Where(x => x.Approved == false && x.IsDeleted == false && x.PageToEdit.Title == approve.EditModel.PageToEdit.Title))
            {
                edit.IsDeleted = true;
                InboxMessage message2 = new InboxMessage()
                {
                    Message = approve.MessageForDeclined,
                    Title = "Your submission has been declined!"
                };
                edit.UserCreated.Inbox.Add(message2);
                _dbContext.Messages.Add(message2);
            }
            _dbContext.SaveChanges();
            if(approve.EditModel.PageToEdit.IsCulture)
            {
                return RedirectToAction("AllCulture", "Page");
            }
            else
            {
                return RedirectToAction("Index", "Page");
            }
        }
        [Authorize(Roles = "Manager,Admin")]
        public IActionResult EditSubmissionDecline(int id)
        {
            ApproveModel model = new ApproveModel();
            model.EditModel = _dbContext.Edits.Where(x => !x.IsDeleted).Include(x => x.PageToEdit).Include(x => x.UserCreated).FirstOrDefault(x => x.ID == id);
            if (model.EditModel == null)
            {
                return RedirectToAction("Index", "Page");
            }
            model.MessageForDeclined = $"Dear {model.EditModel.UserCreated.UserName}, we have reviewed your submission for {(model.EditModel.CreatePage? "creation" : "edit")} of page {model.EditModel.PageToEdit.Title}. Unfortunately, the adminstrators have decided that your submission does not seem appropriate for our platform. Our team thanks you a lot for your decision to contribute to the platform.";
            return View(model);
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public IActionResult EditSubmissionDeclineConfirm(int id, ApproveModel decline)
        {
            EditModel? model = _dbContext.Edits.Where(x => !x.Approved && !x.IsDeleted).Include(x => x.PageToEdit).Include(x => x.UserCreated).FirstOrDefault(x => x.ID == id);
            decline.EditModel = model;
            if (model == null || decline == null)
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
                Message = decline.MessageForDeclined,
                Title = "Your submission has been declined!"
            };
            model.UserCreated.Inbox.Add(message);
            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();
            if(model.PageToEdit.IsCulture)
            {
                return RedirectToAction("AllCulture", "Page");
            }
            else
            {
                return RedirectToAction("Index", "Page");
            }
        }

        [Authorize(Roles = "Manager,Admin")]
        public IActionResult DeleteForumPost(int id)
        { 
            DeclinePostModel model = new DeclinePostModel();
            model.Post = _dbContext.Posts.Where(x => !x.IsDeleted).Include(x => x.User).First(x => x.Id == id);
            if(model.Post == null)
            {
                return RedirectToAction("Index", "Page");
            }
            model.DeclineMessage = $"Dear {model.Post.User.UserName}, we have reviewed your post \"{model.Post.Title}\". However, the adminstrators have decided that your post does not seem appropriate for our platform.";
            return View(model);
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public IActionResult DeleteForumPostConfirm(int id, DeclinePostModel approve)
        {
            ForumQuestion? post = _dbContext.Posts.Where(x => !x.IsDeleted).Include(x => x.Comment).Include(x => x.User).FirstOrDefault(x => x.Id == id);
            PFPModel model = _dbContext.PFPs.FirstOrDefault(x => x.UserName == post.User.UserName);
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
            message.Title = "Your post has been deleted!";
            model.Inbox.Add(message);
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Forum");
        }


        [Authorize(Roles = "Manager,Admin")]
        public IActionResult DeleteComment(int id)
        {
            CommentModel? commentModel = _dbContext.Comments.Where(x => !x.IsDeleted).Include(x => x.User).FirstOrDefault(x => x.Id == id);
            CommentDeleteMessage message = new CommentDeleteMessage()
            {
                Id = id
            };
            if(commentModel == null)
            {
                return RedirectToAction("Index", "Page");
            }
            message.Message = $"Dear {commentModel.User.UserName}, we have reviewed your comment \"{commentModel.Text}\" and we have decided that it does not seem appropriate for our platform. Your comment has since been removed.";
            return View(message);
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public IActionResult DeleteCommentConfirm(int id, CommentDeleteMessage messagePassed)
        {
            CommentModel? comment = _dbContext.Comments.Where(x => !x.IsDeleted).Include(x => x.User).FirstOrDefault(x => x.Id == id);
            comment.PFP = _dbContext.PFPs.FirstOrDefault(x => x.UserName == comment.User.UserName);
            if (comment == null || messagePassed == null || comment.PFP == null)
            {
                return RedirectToAction("Index", "Page");
            }
            comment.IsDeleted = true;
            InboxMessage message1 = new InboxMessage()
            {
                Message = messagePassed.Message,
                Title = "Your comment has been deleted!"
            };
            _dbContext.Messages.Add(message1);
            comment.PFP.Inbox.Add(message1);
            _dbContext.SaveChanges();
            return RedirectToAction("PageViewer", "Page", new { id = comment.PageId });
        }

        [Authorize(Roles = "Manager,Admin")]
        public IActionResult DeleteCommentFromPost(int id)
        {
            CommentModel? comment = _dbContext.Comments.Include(x => x.User).FirstOrDefault(x => x.Id == id);
            CommentPostDeleteMessage message = new CommentPostDeleteMessage()
            {
                Id = id,
            };
            if(comment == null)
            {
                return RedirectToAction("Index", "Page");
            }
            message.Message = $"Dear {comment.User.UserName}, we have reviewd your comment \"{comment.Text}\" and we have decided that it does not seem appropriate for our platform. Your comment has since been removed.";
            return View(message);
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        public IActionResult DeleteCommentFromPostConfirm(int id, CommentPostDeleteMessage messagePassed)
        {
            CommentModel? comment = _dbContext.Comments.Include(x => x.User).FirstOrDefault(x => x.Id == id);
            comment.PFP = _dbContext.PFPs.FirstOrDefault(x => x.UserName == comment.User.UserName);
            if (comment == null || comment.User == null || comment.PFP == null)
            {
                return RedirectToAction("Index", "Page");
            }
            InboxMessage message1 = new InboxMessage()
            {
                Message = messagePassed.Message,
                Title = "Your comment has been deleted!"
            };
            _dbContext.Messages.Add(message1);
            comment.PFP.Inbox.Add(message1);
            comment.IsDeleted = true;
            _dbContext.SaveChanges();
            return RedirectToAction("PostViewer", "Forum", new { id = comment.PostId });
        }

        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> DeleteUserPage(string userName)
        {
            PFPModel? model = _dbContext.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(p => p.UserName == userName);
            IdentityUser? user = _dbContext.Users.FirstOrDefault(u => u.UserName == userName);
            if(model == null || user == null || _manager.UserManager.GetRolesAsync(user).Result.Contains("Admin") || _manager.UserManager.GetRolesAsync(user).Result.Contains("Manager"))
            {
                return RedirectToAction("Index", "Page");
            }
            return View(model);
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteAdminPage(string userName)
        {
            PFPModel? model = _dbContext.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(p => p.UserName == userName);
            IdentityUser? user = _dbContext.Users.FirstOrDefault(u => u.UserName == userName);
            if(model == null || user == null || _manager.UserManager.GetRolesAsync(user).Result.Contains("User") || _manager.UserManager.GetRolesAsync(user).Result.Contains("Manager"))
            {
                return RedirectToAction("Index", "Page");
            }
            return View(model);
        }

        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> DeleteUser(string userName)
        {
            PFPModel? model = _dbContext.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(p => p.UserName == userName);
            IdentityUser? user = _dbContext.Users.FirstOrDefault(u => u.UserName == userName);
            if(user == null || model == null || _manager.UserManager.GetRolesAsync(user).Result.Contains("Admin") || _manager.UserManager.GetRolesAsync(user).Result.Contains("Manager"))
            {
                return RedirectToAction("Index", "Page");
            }
            model.IsDeleted = true;
            await _manager.UserManager.UpdateSecurityStampAsync(user);
            _dbContext.SaveChanges();
            return RedirectToAction("Users", "User");
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteAdminUser(string userName)
        {
            PFPModel? model = _dbContext.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(p => p.UserName == userName);
            IdentityUser? user = _dbContext.Users.FirstOrDefault(u => u.UserName == userName);
            if(user == null || model == null || _manager.UserManager.GetRolesAsync(user).Result.Contains("User") || _manager.UserManager.GetRolesAsync(user).Result.Contains("Manager"))
            {
                return RedirectToAction("Index", "Page");
            }
            model.IsDeleted = true;
            await _manager.UserManager.UpdateSecurityStampAsync(user);
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Page");
        }

        [Authorize(Roles = "Manager")]
        public IActionResult AddToAdmin(string userName)
        {
            IdentityUser? user = _dbContext.Users.FirstOrDefault(x => x.UserName == userName);
            PFPModel? model = _dbContext.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(x => x.UserName == userName);
            AssignAdmin msg = new AssignAdmin();
            if(user == null || model == null)
            {
                return RedirectToAction("Index", "Page");
            }
            msg.User = model.UserName;
            msg.Message = $"Dear {model.UserName}, the My BG team has decided that you seem like an appropriate administrator! You have since been promoted to administrator.";
            if(_manager.UserManager.GetRolesAsync(user).Result.Contains("User"))
            {
                return View(msg);
            }
            else
            {
                return RedirectToAction("Index", "Page");
            }
        }

        [Authorize(Roles = "Manager")]
        public IActionResult RemoveFromAdmin(string userName)
        {
            IdentityUser? user = _dbContext.Users.FirstOrDefault(x => x.UserName == userName);
            PFPModel? model = _dbContext.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(x => x.UserName == userName);
            AssignAdmin msg = new AssignAdmin();
            if(user == null || model == null)
            {
                return RedirectToAction("Index", "Page");
            }
            msg.User = model.UserName;
            msg.Message = $"Dear {model.UserName}, the My BG team has decided that you are not using the admistrator controls in an appropriate way. Therefore, we have decided that the best course of action would be to remove your admin role.";
            if(_manager.UserManager.GetRolesAsync(user).Result.Contains("Admin"))
            {
                return View(msg);
            }
            else
            {
                return RedirectToAction("Index", "Page");
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> ConfirmAdminAssign(string user, string message)
        {
            PFPModel? model = _dbContext.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(x => x.UserName == user);
            IdentityUser? user1 = _dbContext.Users.FirstOrDefault(x => x.UserName == user);
            if(model == null || user == null)
            {
                return RedirectToAction("Index", "Page");
            }
            await _manager.UserManager.RemoveFromRoleAsync(user1, "User");
            await _manager.UserManager.AddToRoleAsync(user1, "Admin");
            InboxMessage msg3 = new InboxMessage()
            {
                UserSource = model,
                Message = message,
                Title = "You have been promoted to admin!"
            };
            await _manager.UserManager.UpdateSecurityStampAsync(user1);
            model.Inbox.Add(msg3);
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Page");
        }

        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> ConfirmAdminRemove(string user, string message)
        {
            PFPModel? model = _dbContext.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(x => x.UserName == user);
            IdentityUser? user1 = _dbContext.Users.FirstOrDefault(x => x.UserName == user);
            if(model == null || user == null)
            {
                return RedirectToAction("Index", "Page");
            }
            await _manager.UserManager.RemoveFromRoleAsync(user1, "Admin");
            await _manager.UserManager.AddToRoleAsync(user1, "User");
            InboxMessage msg3 = new InboxMessage()
            {
                UserSource = model,
                Message = message,
                Title = "You have been removed from the admin role!"
            };
            await _manager.UserManager.UpdateSecurityStampAsync(user1);
            model.Inbox.Add(msg3);
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Page");
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Restore(string userName)
        {
            PFPModel? model = _dbContext.PFPs.Where(x => x.IsDeleted).FirstOrDefault(p => p.UserName == userName);
            IdentityUser? user = _dbContext.Users.FirstOrDefault(u => u.UserName == userName);
            if(model == null || user == null || _manager.UserManager.GetRolesAsync(user).Result.Contains("Manager"))
            {
                return RedirectToAction("Index", "Page");
            }
            return View(model);
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> RestoreConfirm(string userName)
        {
            PFPModel? model = _dbContext.PFPs.Where(x => x.IsDeleted).FirstOrDefault(p => p.UserName == userName);
            IdentityUser? user = _dbContext.Users.FirstOrDefault(u => u.UserName == userName);
            if(user == null || model == null || _manager.UserManager.GetRolesAsync(user).Result.Contains("Manager"))
            {
                return RedirectToAction("Index", "Page");
            }
            model.IsDeleted = false;
            await _manager.UserManager.UpdateSecurityStampAsync(user);
            _dbContext.SaveChanges();
            return RedirectToAction("Users", "User");
        }
    }
}
