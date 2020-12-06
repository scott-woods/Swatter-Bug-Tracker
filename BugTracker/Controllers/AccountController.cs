﻿using BugTracker.Models;
using BugTracker.Models.Email;
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
        private readonly IEmailSender _emailSender;

        public AccountController(ILogger<AccountController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _emailSender = emailSender;
        }

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
                    //var emailMessage = new EmailMessage(new string[] { "scott_woods44@yahoo.com" }, "Test Email", "I hope this works!");
                    //_emailSender.SendEmail(emailMessage);
                    _logger.LogInformation($"User {user.UserName} successfully Logged In.");
                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            //check if email exists
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return RedirectToAction("ForgotPassword");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callback = Url.Action("ResetPassword", "Account", new { token, email = email }, Request.Scheme);
            var emailMessage = new EmailMessage(new string[] { email }, "Swatter Password Reset", $"Please click the following link to reset your password: {callback}");
            _emailSender.SendEmail(emailMessage);
            return RedirectToAction("ResetEmailSentConfirmation");
        }

        [AllowAnonymous]
        public IActionResult ResetEmailSentConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordModel { Token = token, Email = email };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordModel);
            }
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null) RedirectToAction("ResetPasswordConfirmation");
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View();
            }
            return RedirectToAction("ResetPasswordConfirmation");
        }

        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult DemoLogin()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> DemoUserLogin()
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

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string confirmPassword, string firstname, string lastname, string email)
        {
            if (password != confirmPassword)
            {
                return RedirectToAction("Register");
            }
            //Create new User from HTTP form input
            var user = new ApplicationUser
            {
                UserName = username,
                FirstName = firstname,
                LastName = lastname,
                Email = email
            };

            //Add User to Database
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (signInResult.Succeeded)
                {
                    _logger.LogInformation($"New User {user.UserName} successfully Created and Logged In.");
                    return RedirectToAction("Index", "Home");
                }
            }
            //If something went wrong, reload Register page.
            return RedirectToAction("Register");
        }
    }
}
