using MyBG.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBG.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace MyBG.Controllers;

public class ApplicationsController
{
    ApplicationDbContext _ctx;
    UserManager<IdentityUser> _userManager;
    public ApplicationsController(ApplicationDbContext ctx)
    {
        _ctx = ctx;
    }

    [Authorize(Roles = "User")]
    public ActionResult Submit()
    {
        return View(new AssignAdmin());
    }

    [HttpPost]
    [Authorize(Roles = "User")]
    public ActionResult Submit(AssignAdmin message)
    {
        AdminRequest request = new AdminRequest();
        request.UserCreated = _ctx.PFPs.FirstOrDefault(_userManager.GetUserAsync(User).Result.UserName);
        request.Message = message.Message;
        _ctx.Requests.Add(request);
        _ctx.SaveChanges();
        return RedirectToAction("Home")
    }

    [Authorize(Roles = "Admin")]
    public ActionResult ViewPending()
    {
        AdminRequests requests = new AdminRequests();
        requests.Requests = _ctx.Requests.Where(x => !x.Processed).ToList();
        return View(requests);
    }

    [Authorize(Roles = "Admin")]
    public ActionResult ViewRequest(int id)
    {
        AdminRequest? request = _ctx.Requests.Where(x => !x.Processed).FirstOrDefault(x => x.Id == id);
        if(request == null)
        {
            return RedirectToAction("Home");
        }
        return View();
    }

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
    public ActionResult Decline(int id)
    {
        AdminRequestResponse response = new AdminRequestResponse();
        response.Request = _ctx.Requests.Where(x => !x.Processed).FirstOrDefault(x => x.Id == id);
        response.Message = $"Dear {response.Request.UserCreated.UserName}, the My BG team has reviewed your application and has decided that you do not fit as an appropriate administrator. We thank you for your decision to volunteer to be an administrator.";
        if(response == null)
        {
            return RedirectToAction("Home");
        }
        return View(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> Approve(AdminRequestResponse requestResponse)
    {
        InboxMessage msg = new InboxMessage();
        msg.Message = requestResponse.Message;
        msg.Title = "You have been promoted to admin!";
        requestResponse.Request.UserCreated.Inbox.Add(msg);
        IdentityUser user = _ctx.Users.First(x => x.Id == requestResponse.Request.UserCreated.UserName);
        await _userManager.AddToRoleAsync(user, "Admin");
        await _userManager.RemoveFromRoleAsync(user, "User");
        await _userManager.UpdateSecurityStampAsync(user);
        _ctx.SaveChanges();
        return RedirectToAction("Home");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> Decline(AdminRequestResponse requestResponse)
    {
        InboxMessage msg = new InboxMessage();
        msg.Message = requestResponse.Message;
        msg.Title = "Your admin application has been reviewed.";
        requestResponse.Request.UserCreated.Inbox.Add(msg);
        _ctx.SaveChanges();
        return RedirectToAction("Home");
    }
}