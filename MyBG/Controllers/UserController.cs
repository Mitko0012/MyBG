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

    [Authorize]
    public IActionResult UserPage(string userName)
    {
        PFPModel userDataModel = _ctx.PFPs.Where(x => !x.IsDeleted).Include(x => x.Contributions).FirstOrDefault(x => x.UserName == userName);
        if (!ModelState.IsValid || userDataModel == null)
        {
            return RedirectToAction("Index", "Page");
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
        model.Inbox.Reverse();
        return View(model);
    }

    [Authorize]
    public IActionResult InboxMessage(int id)
    {
        InboxMessage msg = _ctx.Messages.Include(x => x.UserSource).FirstOrDefault(x => x.Id == id);
        if(msg == null)
        {
            return RedirectToAction("Index", "Page");
        }
        return View(msg);
    }

    [Authorize]
    public IActionResult Users()
    {
        List<PFPModel> models = new List<PFPModel>();
        models = _ctx.PFPs.Where(p => !p.IsDeleted).ToList();
        Users users = new Users();
        users.AllUsers = new List<IdentityUser>();
        foreach (var model in models)
        {
            users.AllUsers.Add(_ctx.Users.FirstOrDefault(x => x.UserName == model.UserName));
        }
        users.AllUsers.OrderBy((x) => _manager.UserManager.GetRolesAsync(x).Result.Contains("Admin") ? 0 : 1).ToList();
        return View(users);
    }
}