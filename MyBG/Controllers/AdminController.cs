using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
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
            Users users = new Users();
            users.AllUsers = _dbContext.Users.ToList();
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
            PFPModel? user = _dbContext.PFPs.Include(x => x.Contributions).FirstOrDefault(x => x.UserName == _manager.UserManager.GetUserAsync(User).Result.UserName);
            EditModel? model = _dbContext.Edits.Where(x => !x.Approved && !x.IsDeleted).Include(x => x.PageToEdit).FirstOrDefault(x => x.ID == id);
            if(!ModelState.IsValid || model == null || user == null)
            {
                return RedirectToAction("Index");
            }
            model.Approved = true;
            model.PageToEdit.TextBody = model.NewText;
            model.PageIndex = model.PageToEdit.Edits.Where(x => x.Approved).ToList().Count;
            if(model.CreatePage)
            {
                model.PageToEdit.Approved = true;
            }
            _dbContext.SaveChanges();
            foreach (EditModel edit in _dbContext.Edits.Where(x => x.Approved == false && x.PageToEdit.Title == model.PageToEdit.Title))
            {
                edit.IsDeleted = true;
            }
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EditSubmissionDecline(int id)
        {
            EditModel? model = _dbContext.Edits.Where(x => !x.Approved && !x.IsDeleted).Include(x => x.PageToEdit).FirstOrDefault(x => x.ID == id);
            if(!ModelState.IsValid || model == null)
            {
                return RedirectToAction("Index");
            }
            model.IsDeleted = true;
            if(model.CreatePage)
            {
                model.PageToEdit.IsDeleted = true;
            }
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteForumPost(int id)
        {
            ForumQuestion? post = _dbContext.Posts.Where(x => !x.IsDeleted).Include(x => x.Comment).FirstOrDefault(x => x.Id == id);
            if (post == null)
            {
                return RedirectToAction("Index", "Page");
            }
            post.IsDeleted = true;
            foreach(CommentModel comment in post.Comment)
            {
                comment.IsDeleted = true;
            }
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Forum");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteComment(int id)
        {
            CommentModel? comment = _dbContext.Comments.Where(x => !x.IsDeleted).FirstOrDefault(x => x.Id == id);
            if (comment == null)
            {
                return RedirectToAction("Index", "Page");
            }
            comment.IsDeleted = true;
            _dbContext.SaveChanges();
            return RedirectToAction("PageViewer", "Page", new {id = comment.PageId});
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteCommentFromPost(int id)
        {
            CommentModel? comment = _dbContext.Comments.Find(id);
            if (comment == null)
            {
                return RedirectToAction("Index", "Page");
            }
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
