// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyBG.Data;
using MyBG.Models;

namespace MyBG.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private ApplicationDbContext _context;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// 
        public IdentityUser User1 { get; set; }
        public string Username;
        public PFPModel PFP { get; set; }
        [BindProperty]
        public IFormFile FormFile { get; set; }
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var pfp = _context.PFPs.FirstOrDefault(x => x.UserName == userName);
            if (pfp == null)
            {
                pfp = new PFPModel() { UserName = Username }; // Initialize if null
                _context.PFPs.Add(pfp);
                _context.SaveChanges();
            }
            User1 = user;
            PFP = pfp;
            Username = userName; 
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadAsync(await _userManager.GetUserAsync(User));
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await LoadAsync(await _userManager.GetUserAsync(User));
            using (var memoryStream = new MemoryStream())
            {
                FormFile.CopyTo(memoryStream);

                _context.PFPs.FirstOrDefault(x => x.UserName == Username);
                // Upload the file if less than 2 MB
                if (PFP == null)
                {
                    PFP = new PFPModel() { UserName = Username}; // Initialize if null
                    _context.PFPs.Add(PFP);
                }
                if (memoryStream.Length < 2097152)
                {
                    PFP.Image = memoryStream.ToArray();
                }
                else
                {
                    ModelState.AddModelError("File", "The file is too large.");
                }
            }
            _context.SaveChanges();
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
