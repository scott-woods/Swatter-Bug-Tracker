using System;
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
using BugTracker.Models.Email;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger,
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
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
                    //Sends an email upon successful Login
                    var emailMessage = new EmailMessage(new string[] { "scott_woods44@yahoo.com" }, "Test Email", "I hope this works!");
                    _emailSender.SendEmail(emailMessage);
                    _logger.LogInformation($"User {user.UserName} successfully Logged In.");
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Login");
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
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
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
