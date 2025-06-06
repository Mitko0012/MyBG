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
        SignInManager<IdentityUser> _signInManager;

        public PageController(ApplicationDbContext dbContext, UserManager<IdentityUser> manager, SignInManager<IdentityUser> signInManager)
        {
            _context = dbContext;
            _manager = manager;
            _signInManager = signInManager;
        }

        [HttpPost]
        public IActionResult IndexPost(PageModelContainer cont)
        {
            return RedirectToAction("Index", new {displayType = cont.DisplayType, searchString = cont.SearchString, region = (int)cont.RegionSelect, type = (int)cont.DestinationTypeSelect});
        }
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

        [HttpPost]
        public IActionResult AllCulturePost(PageModelContainer cont)
        {
            return RedirectToAction("AllCulture", new { displayType = cont.DisplayType, searchString = cont.SearchString, region = (int)cont.RegionSelect,  type = (int)cont.CultureType });
        }

        public IActionResult Home()
        {
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PageViewer(int id, int? replyCount, string? scroll, string replyString)
        {
            IdentityUser user = new IdentityUser();
            PFPModel userModel = new PFPModel(); 
            PageModel model = _context.Pages.Where(x => !x.IsDeleted && !x.IsCulture).Include(p => p.UsersLiked)
                                        .Include(p => p.Comments)
                                            .ThenInclude(p => p.User)
                                        .Include(p => p.Comments)
                                            .ThenInclude(p => p.LikedUser)
                                        .Include(p => p.Comments)
                                            .ThenInclude(p => p.Replies)
                                            .ThenInclude(p => p.LikedUser)
                                        .Include(p => p.Comments)
                                            .ThenInclude(p => p.Replies)
                                            .ThenInclude(p => p.User)
                                        .Include(p => p.TransportWays)
                                        .FirstOrDefault(p => p.Id == id);
            if(_signInManager.IsSignedIn(User))
            {
                user = await _manager.GetUserAsync(User);
                userModel = _context.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(x => x.UserName == user.UserName);
            }
            if (model == null)
            {
                return RedirectToAction("index");
            }
            model.ReplyString = replyString ?? "d";
            model.Comments = model.Comments.Where(x => !x.IsDeleted).ToList();
            if (replyCount != null && replyCount != 0)
            {
                model.CommenntsToDisplay = replyCount.Value;
            }
            else
            {
                model.CommenntsToDisplay = 5;
            }
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            _page = model;
            if(scroll == null)
            {
                model.Scroll = "0";
            }
            else
            {
                model.Scroll = scroll;
            }
            foreach (var item in model.Comments)
            {
                item.PFP = _context.PFPs.FirstOrDefault(x => x.UserName == item.User.UserName);
                item.Replies = item.Replies.Where(x => !x.IsDeleted).ToList();
                if (_signInManager.IsSignedIn(User) && item.LikedUser.FirstOrDefault(x => x.UserName == user.UserName) != null)
                {
                    item.LikedByUser = true;
                }
                else
                {
                    item.LikedByUser = false;
                }
                foreach(var reply in item.Replies)
                {
                    reply.PFP = _context.PFPs.FirstOrDefault(x => x.UserName == reply.User.UserName);
                    if (_signInManager.IsSignedIn(User) && reply.LikedUser.FirstOrDefault(x => x.UserName == user.UserName) != null)
                    {
                        reply.LikedByUser = true;
                    }
                    else
                    {
                        reply.LikedByUser = false;
                    }
                }
            }
            if (_signInManager.IsSignedIn(User) && model.UsersLiked.FirstOrDefault(x => x.UserName == user.UserName) != null)
            {
                model.LikedByUser = true;
            }
            if (_signInManager.IsSignedIn(User) && userModel.SavedPages.Contains(model.Id))
            {
                model.Saved = true;
            }
            else
            {
                model.Saved = false;
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

        public IActionResult CulturePage(int id, int? replyCount, string? scroll, string replyString)
        {
            IdentityUser user = new IdentityUser();
            PFPModel userModel = new PFPModel(); 
            PageModel model = _context.Pages.Where(x => !x.IsDeleted && x.IsCulture).Include(p => p.UsersLiked)
                                            .Include(p => p.Comments)
                                                .ThenInclude(p => p.User)
                                            .Include(p => p.Comments)
                                                .ThenInclude(p => p.LikedUser)
                                            .Include(p => p.Comments)
                                                .ThenInclude(p => p.Replies)
                                                .ThenInclude(p => p.LikedUser)
                                            .Include(p => p.Comments)
                                                .ThenInclude(p => p.Replies)
                                                .ThenInclude(p => p.User)    
                                            .Include(p => p.TransportWays)
                                            .FirstOrDefault(p => p.Id == id);
            if (model == null)
            {
                return RedirectToAction("index");
            }
            if(_signInManager.IsSignedIn(User))
            {
                user = _manager.GetUserAsync(User).Result;
                userModel = _context.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(x => x.UserName == user.UserName);
            }
            model.Comments = model.Comments.Where(x => !x.IsDeleted).ToList();
            if (replyCount != null && replyCount != 0)
            {
                model.CommenntsToDisplay = (int)replyCount;
            }
            else
            {
                model.CommenntsToDisplay = 5;
            }   
            model.ReplyString = replyString ?? "d";
            if (model == null || user == null)
            {
                return RedirectToAction("Index");
            }
            _page = model;
            if(scroll == null)
            {
                model.Scroll = "0";
            }
            else
            {
                model.Scroll = scroll;
            }
            foreach (var item in model.Comments)
            {
                item.PFP = _context.PFPs.FirstOrDefault(x => x.UserName == item.User.UserName);
                item.Replies = item.Replies.Where(x => !x.IsDeleted).ToList();
                if(_signInManager.IsSignedIn(User))
                {
                    if (item.LikedUser.FirstOrDefault(x => x.UserName == user.UserName) != null)
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
                    reply.PFP = _context.PFPs.FirstOrDefault(x => x.UserName == reply.User.UserName);
                    if(_signInManager.IsSignedIn(User))
                    {
                        if (reply.LikedUser.FirstOrDefault(x => x.UserName == user.UserName) != null)
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
            if (_signInManager.IsSignedIn(User) && model.UsersLiked.FirstOrDefault(x => x.UserName == user.UserName) != null)
            {
                model.LikedByUser = true;
            }   
            else
            {
                model.LikedByUser = false;
            }
            if(_signInManager.IsSignedIn(User))
            {
                if (userModel.SavedPages.Contains(model.Id))
                {
                    model.Saved = true;
                }
                else
                {
                    model.Saved = false;
                }
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
                return View("Create", page);
            }
            foreach(TransportWay transportWay in page.TransportWays)
            {
                _context.TransportWays.Add(transportWay);
            }
            foreach(PageModel existingPage in _context.Pages.Where(x => !x.IsCulture && !x.IsDeleted && x.Approved))
            {
                if(page.Title == existingPage.Title)
                {
                    ModelState.AddModelError("Title", "Another page with the same title exists already.");
                    break;
                }
            }
            using (var memoryStream = new MemoryStream())
            {
                page.PageImage.CopyTo(memoryStream);
                bool isImage = ImageValidator.IsImage(memoryStream);
                if(!isImage)
                {
                    ModelState.AddModelError("FormFile", "The file is not an image file.");
                }
                if(memoryStream.Length < 1)
                {
                    ModelState.AddModelError("FormFile", "The file has no data.");
                }
                if (memoryStream.Length < 2097152)
                {
                    if(isImage && memoryStream.Length >= 1)
                        page.PageImageArr = memoryStream.ToArray();
                }
                else
                {
                    ModelState.AddModelError("PageImage", "The file is too large.");
                }
                if (!ModelState.IsValid)
                {
                    return View("Create", page);
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
        public async Task<IActionResult> PostComment(string comment, int id, int replyCount, PageModel dataModel, string scroll, string replyString)
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
            _context.Comments.Add(comment1);
            model.Comments.Add(comment1);
            _context.SaveChanges();
            if (model.IsCulture)
            {
                return RedirectToAction("CulturePage", new { id = id, replyCount = dataModel.CommenntsToDisplay, scroll = scroll, replyString = replyString });
            }
            return RedirectToAction("PageViewer", new { id = id, replyCount = dataModel.CommenntsToDisplay, scroll = scroll, replyString = replyString });
        }
        
        [Authorize]
        public async Task<IActionResult> LikeComment(int id, int? pageId, PageModel pageModel, string? scroll, string replyString)
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
            if(model.PageId == null)
            {
                model.PageId = pageId;
            }
            if (_context.Pages.Find(model.PageId) != null && _context.Pages.Find(model.PageId).IsCulture)
            {
                return RedirectToAction("CulturePage", new { id = model.PageId, replyCount = pageModel.CommenntsToDisplay, scroll = scroll, replyString = replyString });
            }
            return RedirectToAction("PageViewer", new { id = model.PageId, replyCount = pageModel.CommenntsToDisplay, scroll = scroll, replyString = replyString });
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
                return View("CreateCulture", model);
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
            foreach(PageModel existingPage in _context.Pages.Where(x => x.IsCulture && !x.IsDeleted && x.Approved))
            {
                if(model.Title == existingPage.Title)
                {
                    ModelState.AddModelError("Title", "Another page with the same title exists already.");
                    return View("CreateCulture", model);
                }
            }
            if(model.PageImage == null)
            {
                ModelState.AddModelError("PageImage", "Please upload an image for the page.");
                return View("CreateCulture", model);
            }
            using (var memoryStream = new MemoryStream())
            {
                model.PageImage.CopyTo(memoryStream);
                bool isImage = ImageValidator.IsImage(memoryStream);
                if(!isImage)
                {
                    ModelState.AddModelError("FormFile", "The file is not an image file.");
                }
                if(memoryStream.Length < 1)
                {
                    ModelState.AddModelError("FormFile", "The file has no data.");
                }
                if (memoryStream.Length < 2097152)
                {
                    if(isImage && memoryStream.Length >= 1)
                        model.PageImageArr = memoryStream.ToArray();
                }
                else
                {
                    ModelState.AddModelError("PageImage", "The file is too large.");
                    return View("CreateCulture", model);
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

        public IActionResult CopyrightIcon()
        {
            return View();
        }
    }
}
