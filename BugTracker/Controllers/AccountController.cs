using BugTracker.Models;
using BugTracker.Models.Account;
//using BugTracker.Models.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        //private readonly IEmailSender _emailSender;

        public AccountController(ILogger<AccountController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
            //IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            //_emailSender = emailSender;
        }

        //Register new Account
        [AllowAnonymous]
        public IActionResult Register()
        {
            //ModelState.AddModelError("InvalidPassword", "Invalid Password.");
            var model = new RegisterModel();
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            var passwordValidator = new PasswordValidator<ApplicationUser>();
            var validatorResult = await passwordValidator.ValidateAsync(_userManager, null, registerModel.Password);
            if (!validatorResult.Succeeded)
            {
                ModelState.AddModelError("InvalidPassword", "The Password you entered was Invalid.");
                return View(registerModel);
            }
            if (!ModelState.IsValid)
            {
                return View(registerModel);
            }
            //Create new User from Input
            var user = new ApplicationUser
            {
                UserName = registerModel.Username,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Email = registerModel.Email
            };

            //Add User to Database
            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (result.Succeeded)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, registerModel.Password, false, false);
                if (signInResult.Succeeded)
                {
                    _logger.LogInformation($"New User {user.UserName} successfully Created and Logged In.");
                    return RedirectToAction("Index", "Home");
                }
            }
            //If something went wrong, reload Register page.
            return RedirectToAction("Register");
        }

        
        //Basic Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            var model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                //ModelState.AddModelError("Invalid", "You did not enter a valid Username and Password.");
                return View(loginModel);
            }
            //check if username exists in usermanager
            var user = await _userManager.FindByNameAsync(loginModel.Username);

            if (user != null)
            {
                //sign in
                var signInResult = await _signInManager.PasswordSignInAsync(user, loginModel.Password, loginModel.RememberPassword, false);

                if (signInResult.Succeeded)
                {
                    //Sends an email upon successful Login
                    //var emailMessage = new EmailMessage(new string[] { "scott_woods44@yahoo.com" }, "Test Email", "Test Message");
                    //_emailSender.SendEmail(emailMessage);
                    _logger.LogInformation($"User {user.UserName} successfully Logged In.");
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("ValidUser", "Your Username and/or Password was incorrect.");
            return View(loginModel);
        }

        
        //Login as a Demo User
        [AllowAnonymous]
        public IActionResult DemoLogin()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> DemoSubmitterLogin()
        {
            var user = await _userManager.FindByNameAsync("DemoSubmitter");
            var signInResult = await _signInManager.PasswordSignInAsync(user, "DemoPassword", false, false);

            if (signInResult.Succeeded)
            {
                _logger.LogInformation("User has logged in as a Demo 'Submitter' role.");
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("DemoLogin");
        }

        [AllowAnonymous]
        public async Task<IActionResult> DemoAdminLogin()
        {
            var user = await _userManager.FindByNameAsync("DemoAdmin");
            var signInResult = await _signInManager.PasswordSignInAsync(user, "DemoPassword", false, false);

            if (signInResult.Succeeded)
            {
                _logger.LogInformation("User has logged in as a Demo 'Admin' role.");
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("DemoLogin");
        }

        [AllowAnonymous]
        public async Task<IActionResult> DemoManagerLogin()
        {
            var user = await _userManager.FindByNameAsync("DemoManager");
            var signInResult = await _signInManager.PasswordSignInAsync(user, "DemoPassword", false, false);

            if (signInResult.Succeeded)
            {
                _logger.LogInformation("User has logged in as a Demo 'Manager' role.");
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("DemoLogin");
        }

        [AllowAnonymous]
        public async Task<IActionResult> DemoDeveloperLogin()
        {
            var user = await _userManager.FindByNameAsync("DemoDeveloper");
            var signInResult = await _signInManager.PasswordSignInAsync(user, "DemoPassword", false, false);

            if (signInResult.Succeeded)
            {
                _logger.LogInformation("User has logged in as a Demo 'Developer' role.");
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("DemoLogin");
        }

        
        ////Password resetting
        //[AllowAnonymous]
        //public IActionResult ForgotPassword()
        //{
        //    var model = new ForgotPasswordModel();
        //    return View(model);
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    //check if email exists
        //    var user = await _userManager.FindByEmailAsync(model.Email);

        //    if (user == null)
        //    {
        //        return RedirectToAction("ResetEmailSentConfirmation");
        //    }
        //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //    var callback = Url.Action("ResetPassword", "Account", new { token, email = model.Email }, Request.Scheme);
        //    var emailMessage = new EmailMessage(new string[] { model.Email }, "Swatter Password Reset", $"Please click the following link to reset your password: {callback}");
        //    _emailSender.SendEmail(emailMessage);
        //    return RedirectToAction("ResetEmailSentConfirmation");
        //}

        //[AllowAnonymous]
        //public IActionResult ResetEmailSentConfirmation()
        //{
        //    return View();
        //}

        //[AllowAnonymous]
        //[HttpGet]
        //public IActionResult ResetPassword(string token, string email)
        //{
        //    var model = new ResetPasswordModel { Token = token, Email = email };
        //    return View(model);
        //}

        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        //{
        //    //Check if Password is valid (based on rules set by Identity Package in Startup)
        //    var passwordValidator = new PasswordValidator<ApplicationUser>();
        //    var validatorResult = await passwordValidator.ValidateAsync(_userManager, null, resetPasswordModel.Password);
        //    if (!validatorResult.Succeeded)
        //    {
        //        ModelState.AddModelError("InvalidPassword", "The Password you entered was Invalid.");
        //        return View(resetPasswordModel);
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return View(resetPasswordModel);
        //    }
        //    var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
        //    if (user == null) RedirectToAction("ResetPasswordConfirmation");
        //    var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
        //    if (!resetPassResult.Succeeded)
        //    {
        //        foreach (var error in resetPassResult.Errors)
        //        {
        //            ModelState.TryAddModelError(error.Code, error.Description);
        //        }
        //        return View();
        //    }
        //    return RedirectToAction("ResetPasswordConfirmation");
        //}

        //[AllowAnonymous]
        //public IActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}
    }
}
