using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBG.Data;
using MyBG.Models;

namespace MyBG.Controllers;

public class EditController : Controller
{
    ApplicationDbContext _ctx;
    SignInManager<IdentityUser> _manager;
    public EditController(ApplicationDbContext ctx, SignInManager<IdentityUser> manager)
    {
        _ctx = ctx;
        _manager = manager;
    }

    public IActionResult ViewEdit(int pageId, int editIndex)
    {
        PageModel? page = _ctx.Pages.Include(x => x.Edits).ThenInclude(x => x.UserCreated).FirstOrDefault(x => x.Id == pageId);
        List<EditModel> approvedEdits = page.Edits.Where(x => x.Approved && !x.IsDeleted).ToList();
        EditModel edit = approvedEdits[editIndex - 1];
        if (!ModelState.IsValid || page == null || edit == null)
        {
            return RedirectToAction("Index", "Page");
        }
        return View(edit);
    }

    [Authorize]
    public IActionResult CreateEdit(int pageId)
    {
        EditModel? model = new EditModel();
        PageModel? page = _ctx.Pages.Include(x => x.Edits).FirstOrDefault(x => x.Id == pageId);
        if (!ModelState.IsValid || page == null || model == null)
        {
            return RedirectToAction("Index", "Page");
        }
        model.PageToEdit = page;
        model.PageModelKey = page.Id;
        model.OldText = model.PageToEdit.TextBody;
        model.NewText = model.PageToEdit.TextBody;
        return View(model);
    }
    [Authorize]
    [HttpPost]
    public IActionResult PostEdit(EditModel model, int pageId)
    {
        PageModel? page = _ctx.Pages.Include(x => x.Edits).FirstOrDefault(x => x.Id == pageId);
        model.PageToEdit = page;
        model.PageModelKey = pageId;
        model.OldText = model.PageToEdit.TextBody;
        PFPModel? user = _ctx.PFPs.Include(x => x.Contributions).FirstOrDefault(x => x.UserName == _manager.UserManager.GetUserAsync(User).Result.UserName);
        model.UserCreated = user;
        if (!ModelState.IsValid || page == null || model == null)
        {
            return RedirectToAction("ViewEdits", new { pageId = pageId });
        }
        model.PageToEdit = page;
        model.PageModelKey = pageId;
        model.OldText = page.TextBody;
        _ctx.Edits.Add(model);
        _ctx.SaveChanges();
        return RedirectToAction("ViewEdits", new { pageId = model.PageModelKey });
    }

    public IActionResult ViewEdits(int pageId)
    {
        ViewEditsModel model = new ViewEditsModel();
        PageModel? pageModel = _ctx.Pages.Include(x => x.Edits).FirstOrDefault(x => x.Id == pageId);
        pageModel.Edits = pageModel.Edits.Where(x => !x.IsDeleted).ToList();
        if (!ModelState.IsValid || model == null || pageModel == null)
        {
            return RedirectToAction("Index", "Page");
        }
        model.Edits = pageModel.Edits.Where(x => x.Approved && !x.IsDeleted).ToList();
        return View(model);
    }
}