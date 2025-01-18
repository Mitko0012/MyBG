using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using MyBG.Data;
using MyBG.Models;

namespace MyBG.Controllers
{
    public class PageController : Controller
    {
        PageModel _page = new PageModel();
        ApplicationDbContext _context;
        public PageController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        [Authorize]
        public IActionResult Index()
        {
            PageModelContainer container = new PageModelContainer();
            container.Pages = _context.Submissions.ToList();
            return View(container);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult CreatePost(PageModel page)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid. Errors:");
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }
                return RedirectToAction("Create");
            }
            _context.Submissions.Add(page);
            _context.SaveChanges();
            Console.WriteLine(page.Title);
            return RedirectToAction("Create");
        }
    }
}
