using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBG.Data;
using MyBG.Models;

namespace MyBG.Controllers
{
    public class Admin : Controller
    {
        ApplicationDbContext _dbContext;
        public Admin(ApplicationDbContext context)
        {
            _dbContext = context;
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
        public IActionResult Submissions()
        {
            PageModelContainer cont = new PageModelContainer();
            cont.Pages = _dbContext.Pages.Where((x) => !x.Approved).ToList();
            return View(cont);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult PageAdmin(int id)
        {
            PageModel model = _dbContext.Pages.Include(x => x.TransportWays).FirstOrDefault(x => x.Id == id);
            if (model != null && model.Approved == false)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [Authorize(Roles = "Admin")]
        public IActionResult SubmissionApprove(int id)
        {
            PageModel model = _dbContext.Pages.Find(id);
            model.Approved = true;
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult SubmissionDecline(int id)
        {
            PageModel model = _dbContext.Pages.Find(id);
            _dbContext.Pages.Remove(model);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditSubmissions()
        {
            ViewEditsModel cont = new ViewEditsModel();
            cont.Edits = _dbContext.Edits.Where((x) => !x.Approved).Include(x => x.PageToEdit).ToList();
            return View(cont);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditSubmissionAdmin(int id)
        {
            EditModel submission = _dbContext.Edits.Where(x => !x.Approved).Include(x => x.PageToEdit).FirstOrDefault(x => x.ID == id);
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            return View(submission);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditSubmissionApprove(int id)
        {
            EditModel model = _dbContext.Edits.Where(x => !x.Approved).Include(x => x.PageToEdit).FirstOrDefault(x => x.ID == id);
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            model.Approved = true;
            model.PageToEdit.TextBody = model.NewText;
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult EditSubmissionDecline(int id)
        {
           EditModel model = _dbContext.Edits.Where(x => !x.Approved).FirstOrDefault(x => x.ID == id);
            if(!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            _dbContext.Edits.Remove(model);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }        
    }
}
