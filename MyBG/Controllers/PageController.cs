using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBG.Data;
using MyBG.Migrations;
using MyBG.Models;
using SQLitePCL;

namespace MyBG.Controllers
{
    public class PageController : Controller
    {
        PageModel _page = new PageModel();
        ApplicationDbContext _context;
        UserManager<IdentityUser> _manager;

        public PageController(ApplicationDbContext dbContext, UserManager<IdentityUser> manager)
        {
            _context = dbContext;
            _manager = manager;
        }

        [Authorize]
        [HttpPost]
        public IActionResult IndexPost(PageModelContainer cont)
        {
            return RedirectToAction("Index", new {displayType = cont.DisplayType, searchString = cont.SearchString, region = (int)cont.RegionSelect, type = cont.DisplayType, destinationType = (int)cont.DestinationTypeSelect});
        }
        [Authorize]
        public IActionResult Index(string displayType, string? searchString, int region, int type)

        {
            PageModelContainer container = new PageModelContainer();
            if (displayType == null)
            {
                displayType = "MostLikes";
            }
            else if (displayType != "Search" && displayType != "MostLikes" && displayType != "Region" && displayType != "Destination")
            {
                return RedirectToAction("PageViewer");
            }
            container.RegionSelect = (Regions)region;
            container.SearchString = searchString;
            container.DestinationTypeSelect = (DestinationType)type;
            container.Pages = _context.Pages.Include(x => x.UsersLiked).Include(x => x.TransportWays).Where((x) => x.Approved && x.IsCulture == false).ToList();
            switch (displayType)
            {
                case "Search":
                    if (searchString == null)
                    {
                        searchString = "";
                    }
                    container.Pages = container.Pages.Where(x => x.Title.Contains(searchString)).ToList();
                    break;
                case "MostLikes":
                    container.Pages = container.Pages.OrderBy(x => -x.UsersLiked.Count).ToList();
                    break;
                case "Region":
                    container.Pages = container.Pages.Where(x => x.Regions == container.RegionSelect).ToList();
                    break;
                case "Destination":
                    container.Pages = container.Pages.Where(x => x.DestinationType == container.DestinationTypeSelect).ToList();
                    break;
            }
            return View(container);
        }

        [Authorize]
        public IActionResult AllCulture(string displayType, string? searchString, int region, int type)
        {
            PageModelContainer container = new PageModelContainer();
            if (displayType == null)
            {
                displayType = "MostLikes";
            }
            else if (displayType != "Search" && displayType != "MostLikes" && displayType != "CultureType")
            {
                return RedirectToAction("PageViewer");
            }
            container.Pages = _context.Pages.Include(x => x.UsersLiked).Include(x => x.TransportWays).Where((x) => x.Approved && x.IsCulture).ToList();
            container.SearchString = searchString;
            container.CultureType = (CultureType)type;
            container.RegionSelect = (Regions)region;
            switch (displayType)
            {
                case "Search":
                    if (searchString == null)
                    {
                        searchString = "";
                    }
                    container.Pages = container.Pages.Where(x => x.Title.Contains(searchString)).ToList();
                    break;
                case "MostLikes":
                    container.Pages = container.Pages.OrderBy(x => -x.UsersLiked.Count).ToList();
                    break;
                case "CultureType":
                    container.Pages = container.Pages.Where(x => x.CultureType == container.CultureType).ToList();
                    break;
            }
            return View(container);
        }

        [Authorize]
        [HttpPost]
        public IActionResult AllCulturePost(PageModelContainer cont)
        {
            return RedirectToAction("AllCulture", new { displayType = cont.DisplayType, searchString = cont.SearchString, region = (int)cont.RegionSelect,  type = (int)cont.CultureType });
        }

        [Authorize]
        public IActionResult Home()
        {
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> PageViewer(int id, int? replyCount)
        {
            IdentityUser user = await _manager.GetUserAsync(User);
            PageModel model = _context.Pages.Where(x => !x.IsDeleted && !x.IsCulture).Include(p => p.UsersLiked)
                                            .Include(p => p.Comments)
                                                .ThenInclude(p => p.User)
                                            .Include(p => p.Comments)
                                                .ThenInclude(p => p.LikedUser)
                                            .Include(p => p.TransportWays)
                                            .FirstOrDefault(p => p.Id == id);
            if (model == null || user == null)
            {
                return RedirectToAction("index");
            }
            model.Comments = model.Comments.Where(x => !x.IsDeleted).ToList();
            if (replyCount != null)
            {
                model.CommenntsToDisplay = replyCount.Value;
            }
            else
            {
                model.CommenntsToDisplay = 10;
            }
            if (replyCount != null)
            {
                model.CommenntsToDisplay = (int)replyCount;
            }
            if (model == null || user == null)
            {
                return RedirectToAction("Index");
            }
            _page = model;
            foreach (var item in model.Comments)
            {
                item.PFP = _context.PFPs.FirstOrDefault(x => x.UserName == item.User.UserName);
            }
            if (model.UsersLiked.FirstOrDefault(x => x.UserName == user.UserName) != null)
            {
                model.LikedByUser = true;
            }
            else
            {
                model.LikedByUser = false;
            }
            if (model != null && model.Approved == true)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [Authorize]
        public IActionResult CulturePage(int id, int? replyCount)
        {
            IdentityUser user = _manager.GetUserAsync(User).Result;
            PageModel model = _context.Pages.Where(x => !x.IsDeleted && x.IsCulture).Include(p => p.UsersLiked)
                                            .Include(p => p.Comments)
                                                .ThenInclude(p => p.User)
                                            .Include(p => p.Comments)
                                                .ThenInclude(p => p.LikedUser)
                                            .Include(p => p.TransportWays)
                                            .FirstOrDefault(p => p.Id == id);
           if (model == null || user == null)
            {
                return RedirectToAction("index");
            }
            model.Comments = model.Comments.Where(x => !x.IsDeleted).ToList();
            if (replyCount != null)
            {
                model.CommenntsToDisplay = replyCount.Value;
            }
            else
            {
                model.CommenntsToDisplay = 10;
            }   
            if (replyCount != null)
            {
                model.CommenntsToDisplay = (int)replyCount;
            }
            if (model == null || user == null)
            {
                return RedirectToAction("Index");
            }
            _page = model;
            foreach (var item in model.Comments)
            {
                item.PFP = _context.PFPs.FirstOrDefault(x => x.UserName == item.User.UserName);
            }
            if (model.UsersLiked.FirstOrDefault(x => x.UserName == user.UserName) != null)
            {
                model.LikedByUser = true;
            }   
            else
            {
                model.LikedByUser = false;
            }
            if (model != null && model.Approved == true)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }
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
            if (!ModelState.IsValid || page == null)
            {
                return RedirectToAction("Create");
            }
            foreach(TransportWay transportWay in page.TransportWays)
            {
                _context.TransportWays.Add(transportWay);
            }
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
                    return RedirectToAction("Create");
                }
                PFPModel user = _context.PFPs.Include(x => x.Contributions).FirstOrDefault(x => x.UserName == _manager.GetUserAsync(User).Result.UserName);
                EditModel firstEdit = new EditModel()
                {
                    Approved = false,
                    OldText = "",
                    NewText = page.TextBody,
                    PageToEdit = page,
                    PageModelKey = page.Id,
                    CreatePage = true,
                    UserCreated = user,
                };
                page.Edits.Add(firstEdit);
                _context.Pages.Add(page);
                firstEdit.PFPKey = user.Id;
                user.Contributions.Add(firstEdit);
                _context.Edits.Add(firstEdit);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LikePost(int id, int replyCount)
        {
            IdentityUser startUser = await _manager.GetUserAsync(User);
            PFPModel user = _context.PFPs.Where(x => !x.IsDeleted).Include(x => x.PagesLiked)
                                         .FirstOrDefault(x => x.UserName == startUser.UserName);
            PageModel model = _context.Pages.Where(x => !x.IsDeleted).Include(p => p.UsersLiked)
                                            .Include(p => p.Comments)
                                            .FirstOrDefault(p => p.Id == id);
            if (model == null || user == null)
            {
                return RedirectToAction("PageViewer", new { id = id });
            }
            if (model.UsersLiked.Contains(user))
            {
                model.UsersLiked.Remove(user);
                model.LikedByUser = false;
            }
            else
            {
                model.UsersLiked.Add(user);
                model.LikedByUser = true;
            }
            _context.SaveChanges();
            if(model.IsCulture)
            {
                return RedirectToAction("CulturePage", new { id = id, replyCount = replyCount });
            }
            return RedirectToAction("PageViewer", new { id = id, replyCount = replyCount });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostComment(string comment, int id, int replyCount)
        {
            IdentityUser user = await _manager.GetUserAsync(User);
            PageModel model = _context.Pages.Where(x => !x.IsDeleted).Include(p => p.UsersLiked)
                                            .Include(p => p.Comments)
                                            .FirstOrDefault(p => p.Id == id);
            PFPModel pfp = _context.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(x => x.UserName == user.UserName);
            CommentModel comment1 = new CommentModel() { Text = comment, User = user, PageId = id };
            if (model == null || user == null || comment1 == null || pfp == null)
            {
                return RedirectToAction("PageViewer", new { id = id, replyCount = model.CommenntsToDisplay });
            }
            if (!ModelState.IsValid)
            {
                return RedirectToAction("PageViewer", new { id = id, replyCount = model.CommenntsToDisplay });
            }
            _context.Comments.Add(comment1);
            model.Comments.Add(comment1);
            _context.SaveChanges();
            if (model.IsCulture)
            {
                return RedirectToAction("CulturePage", new { id = id, replyCount = replyCount });
            }
            return RedirectToAction("PageViewer", new { id = id, replyCount = replyCount });
        }
        [Authorize]
        public async Task<IActionResult> LikeComment(int id, int replyCount)
        {
            IdentityUser sourceUser = await _manager.GetUserAsync(User);
            PFPModel user = _context.PFPs.Where(x => !x.IsDeleted).Include(p => p.CommentsLiked)
                            .FirstOrDefault(x => x.UserName == sourceUser.UserName);
            CommentModel model = _context.Comments.Where(x => !x.IsDeleted).Include(p => p.LikedUser)
                                            .FirstOrDefault(p => p.Id == id);
            if (user == null || model == null)
            {
                return RedirectToAction("Index");
            }
            if (!model.LikedUser.Any(u => u.Id == user.Id))
            {
                model.LikedUser.Add(user);
            }
            else
            {
                model.LikedUser.Remove(user);
            }
            _context.SaveChanges();
            if (_context.Pages.Find(model.PageId).IsCulture)
            {
                return RedirectToAction("CulturePage", new { id = id, replyCount = replyCount });
            }
            return RedirectToAction("PageViewer", new { id = model.PageId, replyCount = replyCount });
        }

        [Authorize]
        public IActionResult CreateCulture()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateCulture(PageModel model)
        {
            if(model == null)
            {
                return RedirectToAction("CreateCulture");
            }

            model.IsCulture = true;
            model.TransportWays = new List<TransportWay>();
            model.TransportWays.Add(new TransportWay()
            {
                TransportOrigin = "",
                TransportTimeHours = 0,
                TransportTimeMinutes = 0
            });
            model.Regions = 0;
            using (var memoryStream = new MemoryStream())
            {
                model.PageImage.CopyTo(memoryStream);

                // Upload the file if less than 2 MB
                if (memoryStream.Length < 2097152)
                {
                    model.PageImageArr = memoryStream.ToArray();
                }
                else
                {
                    ModelState.AddModelError("File", "The file is too large.");
                    if (!ModelState.IsValid)
                    {
                        return RedirectToAction("CreateCulture");
                    }
                }
                PFPModel user = _context.PFPs.Include(x => x.Contributions).FirstOrDefault(x => x.UserName == _manager.GetUserAsync(User).Result.UserName);
                EditModel firstEdit = new EditModel()
                {
                    Approved = false,
                    OldText = "",
                    NewText = model.TextBody,
                    PageToEdit = model,
                    PageModelKey = model.Id,
                    CreatePage = true,
                    UserCreated = user,
                };
                model.VerifyTransport = "";
                model.Edits.Add(firstEdit);
                _context.Pages.Add(model);
                firstEdit.PFPKey = user.Id;
                user.Contributions.Add(firstEdit);
                _context.Edits.Add(firstEdit);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}
