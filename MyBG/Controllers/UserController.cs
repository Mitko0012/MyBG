using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Composition.Convention;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBG.Data;
using MyBG.Models;
using SQLitePCL;

namespace MyBG.Controllers;

public class UserController : Controller
{
    ApplicationDbContext _ctx;
    SignInManager<IdentityUser> _manager;
    public UserController(ApplicationDbContext ctx, SignInManager<IdentityUser> manager)
    {
        _ctx = ctx;
        _manager = manager;
    }

    public IActionResult UserPage(string userName)
    {
        PFPModel userDataModel = _ctx.PFPs.Where(x => !x.IsDeleted).Include(x => x.Contributions).Include(x => x.Inbox).FirstOrDefault(x => x.UserName == userName);
        if (!ModelState.IsValid || userDataModel == null)
        {
            userDataModel = new PFPModel();
            userDataModel.UserName = userName;
            return View("NotFound", userDataModel);
        }
        userDataModel.Contributions = userDataModel.Contributions.Where(x => x.Approved && !x.IsDeleted).ToList();
        return View(userDataModel);
    }

    [Authorize]
    public IActionResult ViewContributions(string userName)
    {
        PFPModel userDataModel = _ctx.PFPs.Where(x => !x.IsDeleted).Include(x => x.Contributions).ThenInclude(x => x.PageToEdit).FirstOrDefault(x => x.UserName == userName);
        if (!ModelState.IsValid || userDataModel == null)
        {
            return RedirectToAction("Index", "Page");
        }
        UserContribsModel model = new UserContribsModel()
        {
            Edits = userDataModel.Contributions.Where(x => x.Approved).ToList(),
            UserName = userDataModel.UserName
        };
        return View(model);
    }

    [Authorize]
    public IActionResult Inbox()
    {
        PFPModel model = _ctx.PFPs.Where(x => !x.IsDeleted).Include(x => x.Inbox).FirstOrDefault(x => x.UserName == _manager.UserManager.GetUserAsync(User).Result.UserName);
        if(model == null)
        {
            return RedirectToAction("Index", "Page");
        }
        foreach(InboxMessage msg in model.Inbox.Where(x => !x.IsRead))
        {
            msg.IsRead = true;
        }
        model.Inbox.Reverse();
        _ctx.SaveChanges();
        return View(model);
    }

    [Authorize]
    public IActionResult InboxMessage(int id)
    {
        InboxMessage msg = _ctx.Messages.Include(x => x.UserSource).Where(x => x.UserSource.UserName == _manager.UserManager.GetUserAsync(User).Result.UserName).FirstOrDefault(x => x.Id == id);
        if(msg == null)
        {
            return RedirectToAction("Index", "Page");
        }
        return View(msg);
    }

    public IActionResult Users()
    {
        List<PFPModel> models = _ctx.PFPs.Where(p => !p.IsDeleted).ToList();
        List<PFPModel> deletedModels = _ctx.PFPs.Where(p => p.IsDeleted).ToList();
        Users users = new Users();
        users.AllUsers = new List<IdentityUser>();
        users.DeletedUsers = new List<IdentityUser>();
        foreach (var model in models)
        {
            users.AllUsers.Add(_ctx.Users.FirstOrDefault(x => x.UserName == model.UserName));
        }
        foreach(var model in deletedModels)
        {
            users.DeletedUsers.Add(_ctx.Users.FirstOrDefault(x => x.UserName == model.UserName));
        }
        users.AllUsers = users.AllUsers.OrderBy((x) => _manager.UserManager.GetRolesAsync(x).Result.Contains("Manager") ? 0 : _manager.UserManager.GetRolesAsync(x).Result.Contains("Admin") ? 1 : 2).ToList();
        users.DeletedUsers = users.DeletedUsers.OrderBy((x) => _manager.UserManager.GetRolesAsync(x).Result.Contains("Admin") ? 0 : 1).ToList();
        return View(users);
    }

    [Authorize]
    public IActionResult SavePage(int pageId)
    {
        PageModel? model = _ctx.Pages.Where(x => !x.IsDeleted && x.Approved).FirstOrDefault(x => x.Id == pageId);
        PFPModel? pfp = _ctx.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(x => x.UserName == _manager.UserManager.GetUserAsync(User).Result.UserName);
        if(model == null || pfp == null)
        {
            return RedirectToAction("Index", "Page");
        }
        if(pfp.SavedPages.Contains(model.Id))
        {
            pfp.SavedPages.Remove(model.Id);
        } 
        else
        {
            pfp.SavedPages.Add(model.Id);
        }
        _ctx.SaveChanges();
        if(model.IsCulture)
        {
            return RedirectToAction("CulturePage", "Page", new {id = pageId});
        }
        else
        {
            return RedirectToAction("PageViewer", "Page", new {id = pageId});
        }
    }
    [Authorize]
    public IActionResult ViewSaved()
    {
        PFPModel? model = _ctx.PFPs.Where(x => !x.IsDeleted).FirstOrDefault(x => x.UserName == _manager.UserManager.GetUserAsync(User).Result.UserName);
        if(model == null)
        {
            return RedirectToAction("Index", "Page");
        }
        SavedPages page = new SavedPages();
        List<PageModel> pages = new List<PageModel>();
        foreach(int i in model.SavedPages)
        {
            pages.Add(_ctx.Pages.Find(i));
        }
        page.Pages = pages;
        return View(page);
    }
}