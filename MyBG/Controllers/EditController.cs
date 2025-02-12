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
        PageModel page = _ctx.Pages.Include(x => x.Edits).FirstOrDefault(x => x.Id == pageId);
        List<EditModel> approvedEdits = page.Edits.Where(x => x.Approved).ToList();
        EditModel edit = approvedEdits[editIndex - 1];
        edit.PageIndex = editIndex;
        if(!ModelState.IsValid)
        {
            return RedirectToAction("Index", "Page");
        }
        return View(edit);
    }
    
    [Authorize]
    public IActionResult CreateEdit(int pageId)
    {
        EditModel edit = new EditModel();
        edit.PageToEdit = _ctx.Pages.FirstOrDefault(x => x.Id == pageId);
        edit.PageModelKey = pageId;
        edit.OldText = edit.PageToEdit.TextBody;
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
        PageModel page = _ctx.Pages.Include(x => x.Edits).FirstOrDefault(x => x.Id == pageId);
        page.Edits.Add(model);
        if(!ModelState.IsValid)
        {
            Console.WriteLine(ModelState.Values.SelectMany(x => x.Errors));
            return RedirectToAction("ViewEdits", new {id = model.PageModelKey});
        }
        model.PageToEdit = page;
        model.PageModelKey = pageId;
        model.OldText = page.TextBody;
        _ctx.Edits.Add(model);
        _ctx.SaveChanges();
        return RedirectToAction("ViewEdits", new {pageId = model.PageModelKey});
    }

    [Authorize]
    public IActionResult ViewEdits(int pageId)
    {
        ViewEditsModel model = new ViewEditsModel();
        PageModel pageModel = _ctx.Pages.Include(x => x.Edits).FirstOrDefault(x => x.Id == pageId);
        model.Edits = pageModel.Edits.Where(x => x.Approved).ToList();
        if(!ModelState.IsValid)
        {
            return RedirectToAction("Index", "Page");
        }
        return View(model);
    }
}