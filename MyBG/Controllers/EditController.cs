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

namespace MyBG.Controllers;

public class EditController : Controller
{
    ApplicationDbContext _ctx;
    public EditController(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    [Authorize]
    public IActionResult ViewEdit(int pageId, int editIndex)
    {
        PageModel page = _ctx.Pages.Include(x => x.Edits).FirstOrDefault(x => x.Id == pageId)
        List<EditController> approvedEdits = page.Edits.Where(x => x.Approved).ToList();
        Edit edit = approvedEdits[editIndex - 1];
        if(!ModelState.IsValid)
        {
            return RedirectToAction("Index", "Page");
        }
        return View(edit);
    }

    [Authorize]
    [HttpPost]
    public IActionResult PostEdit(EditModel model, int pageId)
    {
        PageModel page = _ctx.Pages.Include(x => x.Edits).FirstOrDefault(x => x.Id == pageId)
        model.PageModelKey = page.Id;
        model.PageToEdit = page;
        page.Edits.add(model);
        if(!ModelState.IsValid)
        {
            return RedirectToAction("ViewEdits", new {id = pageId});
        }
        _ctx.SaveChanges();
        RedirectToAction("ViewEdits", new {id = pageId});
    }

    [Authorize]
    public IActionResult ViewEdits(int pageId)
    {
        ViewEditsModel model = new ViewEditsModel();
        PageModel pageModel = _ctx.Pages.Include(x => x.Edits).FirstOrDefault(x => x.Id == pageID);
        model.Edits = pageModel.Edits.Where(x => x.Approved).ToList();
        if(!ModelState.IsValid)
        {
            return RedirectToAction("Index", "Page");
        }
        return View(model);
    }
}