using IdentityMVCDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityMVCDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<DemoUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<DemoUser> _claimsPrincipalFactory;
        private readonly SignInManager<DemoUser> _signInManager;

        public HomeController(ILogger<HomeController> logger,
            UserManager<DemoUser> userManager,
            IUserClaimsPrincipalFactory<DemoUser> claimsPrincipalFactory,
            SignInManager<DemoUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //Protects from other websites using our POST endpoint
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    user = new DemoUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = model.UserName,
                        Email = model.UserName
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationEmail = Url.Action("ConfirmEMailAddress", "Home",
                            new {token = token, email = user.Email}, Request.Scheme);
                        System.IO.File.WriteAllText("confirmationLink.txt", confirmationEmail);
                    }
                }

                return View("Success");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAddress(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return View("Success");
                }
            }

            return View("Error");
        }
        //[HttpGet]
        //public IActionResult Login()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginModel model)
        //{

        //}

        [Authorize]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                #region UserManager

                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "Email is not confirmed.");
                        return View();
                    }

                    var principal = await _claimsPrincipalFactory.CreateAsync(user);

                    await HttpContext.SignInAsync("Identity.Application", principal);

                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("","Invalid UserName or Password.");

                #endregion

                #region SignInManager


                //var signInResult = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);

                //if (signInResult.Succeeded)
                //{
                //    return RedirectToAction("Index");
                //}

                //ModelState.AddModelError("", "Invalid UserName or Password");

                #endregion


            }

            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var resetUrl = Url.Action("ResetPassword", "Home",
                        new {token = token, email = user.Email}, Request.Scheme);

                    System.IO.File.WriteAllText("resetLink.txt", resetUrl);
                }
                else
                {
                    //email user and inform them that they do not have an account
                }

                return View("Success");
            }

            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            return View(new ResetPasswordModel {Token = token, Email = email});
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        return View();
                    }

                    return View("Success");
                }

                ModelState.AddModelError("", "Invalid Request");
            }

            return View();
        }
    }
}