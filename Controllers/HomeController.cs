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
using SQLitePCL;

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
            using (var memoryStream = new MemoryStream())
            {
                page.PageImage.CopyTo(memoryStream);

                // Upload the file if less than 2 MB
                if (memoryStream.Length < 2097152)
                {
                    page.PageImageArr = memoryStream.ToArray();
                }
                else
                {
                    ModelState.AddModelError("File", "The file is too large.");
                }
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
                return RedirectToAction("Index");
            }
        }
    }
}
