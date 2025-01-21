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
            cont.Pages = _dbContext.Submissions.ToList();
            return View(cont);
        }
    }
}
