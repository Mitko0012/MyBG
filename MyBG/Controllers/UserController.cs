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
        return View(model);
    }
}