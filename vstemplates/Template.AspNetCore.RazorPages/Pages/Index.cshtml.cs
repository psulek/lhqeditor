using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Template.AspNetCore.RazorPages.Models;

namespace Template.AspNetCore.RazorPages.Pages
{
    public class IndexModel : PageModel
    {
        public string GetWelcomeMessage()
        {
            // LHQ: Returns localized text with input parameters
            return StringsModel.Messages.WelcomeMessage(Model.Name, Model.Surname, Model.Username);
        }

        public IActionResult OnGet(string userName, string name, string surname)
        {
            Model = new HomeViewModel
            {
                Username = userName,
                Name = name,
                Surname = surname
            };
            return Page();
        }

        public IActionResult OnGetSetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        [BindProperty]
        public HomeViewModel Model { get; set; }

        public bool HasSubmitData
        {
            get
            {
                return Model != null && (!string.IsNullOrEmpty(Model.Name) || !string.IsNullOrEmpty(Model.Surname));
            }
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                return RedirectToPage(new
                {
                    userName = Model.Username,
                    name = Model.Name,
                    surname = Model.Surname
                });
            }

            Model = new HomeViewModel();
            return Page();
        }
    }
}
