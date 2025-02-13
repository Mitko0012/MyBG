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
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditSubmissions()
        {
            ViewEditsModel cont = new ViewEditsModel();
            cont.Edits = _dbContext.Edits.Where((x) => !x.Approved).Include(x => x.PageToEdit).Include(x => x.UserCreated).ToList();
            return View(cont);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditSubmissionAdmin(int id)
        {
            EditModel submission = _dbContext.Edits.Where(x => !x.Approved).Include(x => x.PageToEdit).Include(x => x.UserCreated).FirstOrDefault(x => x.ID == id);
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            return View(submission);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditSubmissionApprove(int id)
        {
            PFPModel user = _dbContext.PFPs.Include(x => x.Contributions).FirstOrDefault(x => x.UserName == _manager.UserManager.GetUserAsync(User).Result.UserName);
            EditModel model = _dbContext.Edits.Where(x => !x.Approved).Include(x => x.PageToEdit).FirstOrDefault(x => x.ID == id);
            if(!ModelState.IsValid)
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
                _dbContext.Edits.Remove(edit);
            }
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EditSubmissionDecline(int id)
        {
            EditModel model = _dbContext.Edits.Where(x => !x.Approved).Include(x => x.PageToEdit).FirstOrDefault(x => x.ID == id);
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            _dbContext.Edits.Remove(model);
            _dbContext.Pages.Remove(model.PageToEdit);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }        
    }
}
