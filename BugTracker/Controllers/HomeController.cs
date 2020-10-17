﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using BugTracker.Models;
using Microsoft.AspNetCore.Identity;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool rememberPasswordCheck)
        {
            //check if username exists in usermanager
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                //sign in
                var signInResult = await _signInManager.PasswordSignInAsync(user, password, rememberPasswordCheck, false);

                if (signInResult.Succeeded)
                {
                    _logger.LogInformation($"User {user.UserName} successfully Logged In.");
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> DemoLogin()
        {
            var user = await _userManager.FindByNameAsync("Demo");
            var signInResult = await _signInManager.PasswordSignInAsync(user, "DemoPassword", false, false);

            if (signInResult.Succeeded)
            {
                _logger.LogInformation("User has logged in as a Demo User.");
                return RedirectToAction("Index");
            }

            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }

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
                    return RedirectToAction("Index");
                }
            }
            //If something went wrong, reload Register page.
            return RedirectToAction("Register");
        }

        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation($"User {User.Identity.Name} has logged out.");
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
