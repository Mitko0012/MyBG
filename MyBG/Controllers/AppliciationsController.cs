using MyBG.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBG.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace MyBG.Controllers;

public class ApplicationsController : Controller
{
    ApplicationDbContext _ctx;
    UserManager<IdentityUser> _userManager;
    public ApplicationsController(ApplicationDbContext ctx, UserManager<IdentityUser> userManager)
    {
        _ctx = ctx;
        _userManager = userManager;
    }

    [Authorize(Roles = "User")]
    public ActionResult Submit()
    {
        return View(new AssignAdmin()
        {
            User = _userManager.GetUserAsync(User).Result.UserName
        });
    }

    [Authorize(Roles = "User")]
    [HttpPost]
    public ActionResult Submit(string message)
    {
        AdminRequest request = new AdminRequest();
        request.UserCreated = _ctx.PFPs.FirstOrDefault(x => x.UserName == _userManager.GetUserAsync(User).Result.UserName);
        request.Message = message;
        _ctx.Requests.Add(request);
        _ctx.SaveChanges();
        return RedirectToAction("Index", "Page");
    }

    [Authorize(Roles = "Manager")]
    public ActionResult ViewPending()
    {
        AdminRequests requests = new AdminRequests();
        requests.Requests = _ctx.Requests.Include(x => x.UserCreated).Where(x => !x.Processed).ToList();
        return View(requests);
    }

    [Authorize(Roles = "Manager")]
    public ActionResult ViewRequest(int id)
    {
        AdminRequest? request = _ctx.Requests.Where(x => !x.Processed).Include(x => x.UserCreated).FirstOrDefault(x => x.Id == id);
        if(request == null)
        {
            return RedirectToAction("Index", "Page");
        }
        return View(request);
    }

    [Authorize(Roles = "Manager")]
    public ActionResult Approve(int id)
    {
        AdminRequestResponse response = new AdminRequestResponse();
        response.Request = _ctx.Requests.Include(x => x.UserCreated).Where(x => !x.Processed).FirstOrDefault(x => x.Id == id);
        response.Message = $"Dear {response.Request.UserCreated.UserName}, the My BG team has decided that you seem like an appropriate administrator! You have since been promoted to administrator.";
        if(response == null)
        {
            return RedirectToAction("Home");
        }
        return View(response);
    }

    [Authorize(Roles = "Manager")]
    public ActionResult Decline(int id)
    {
        AdminRequestResponse response = new AdminRequestResponse();
        response.Request = _ctx.Requests.Where(x => !x.Processed).Include(x => x.UserCreated).FirstOrDefault(x => x.Id == id);
        response.Message = $"Dear {response.Request.UserCreated.UserName}, the My BG team has reviewed your application and has decided that you do not fit as an appropriate administrator. We thank you for your decision to volunteer to be an administrator.";
        if(response == null)
        {
            return RedirectToAction("Home");
        }
        return View(response);
    }

    [Authorize(Roles = "Manager")]
    [HttpPost]
    public async Task<ActionResult> Approve(string message, AdminRequest? request)
    {
        InboxMessage msg = new InboxMessage();
        msg.Message = message;
        msg.Title = "You have been promoted to admin!";
        request = _ctx.Requests.Where(x => !x.Processed).Include(x => x.UserCreated).FirstOrDefault(x => x.Id == request.Id);
        if(request == null)
        {
            return RedirectToAction("Index", "Page");
        }
        request.Processed = true;
        request.UserCreated.Inbox.Add(msg);
        IdentityUser user = _ctx.Users.First(x => x.UserName == request.UserCreated.UserName);
        await _userManager.AddToRoleAsync(user, "Admin");
        await _userManager.RemoveFromRoleAsync(user, "User");
        await _userManager.UpdateSecurityStampAsync(user);
        _ctx.SaveChanges();
        return RedirectToAction("Index", "Page");
    }

    [Authorize(Roles = "Manager")]
    [HttpPost]
    public async Task<ActionResult> Decline(string message, AdminRequest? request)
    {
        InboxMessage msg = new InboxMessage();
        msg.Message = message;
        msg.Title = "Your admin application has been reviewed.";
        request = _ctx.Requests.Where(x => !x.Processed).Include(x => x.UserCreated).FirstOrDefault(x => x.Id == request.Id);
        if(request == null)
        {
            return RedirectToAction("Index", "Page");
        }
        request.Processed = true;
        request.UserCreated.Inbox.Add(msg);
        _ctx.SaveChanges();
        return RedirectToAction("Index", "Page");
    }
}