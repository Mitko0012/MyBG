using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
    public UserController(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    [Authorize]
    public IActionResult UserPage(string userName)
    {
        PFPModel userDataModel = _ctx.PFPs.FirstOrDefault(x => x.UserName == userName);
        if (!ModelState.IsValid || userDataModel == null)
        {
            return RedirectToAction("Index", "Page");
        }
        return View(userDataModel);
    }

    [Authorize]
    public IActionResult ViewContributions(string userName)
    {
        PFPModel userDataModel = _ctx.PFPs.Include(x => x.Contributions).ThenInclude(x => x.PageToEdit).FirstOrDefault(x => x.UserName == userName);
        if (!ModelState.IsValid || userDataModel == null)
        {
            return RedirectToAction("Index", "Page");
        }
        UserContribsModel model = new UserContribsModel()
        {
            Edits = userDataModel.Contributions,
            UserName = userDataModel.UserName
        };
        return View(model);
    }
}