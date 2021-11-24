using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Template.AspNetCore.MVC.VS2015.Models;

namespace Template.AspNetCore.MVC.VS2015.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string userName = null, string name = null, string surname = null)
        {
            return View(new HomeViewModel
            {
                Username = userName,
                Name = name,
                Surname = surname
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ActionName("Index")]
        public IActionResult IndexPost(HomeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", new
                {
                    userName = viewModel.Username,
                    name = viewModel.Name,
                    surname = viewModel.Surname
                });
            }

            viewModel = new HomeViewModel();
            return View("Index", viewModel);
        }

        [HttpGet]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
