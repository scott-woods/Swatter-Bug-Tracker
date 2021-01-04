using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.AspNetCore.Identity;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using BugTracker.Models.Email;
using System.Security.Claims;
using BugTracker.AuthorizationRequirements;
using BugTracker.Services;

namespace BugTracker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Email Config
            var emailConfig = Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddScoped<IEmailSender, EmailSender>();

            //Add Data Model Services
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IProjectServices, ProjectServices>();

            //Database Config
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("SwatterConnection")));

            //Identity Config
            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 8;
                config.Password.RequireDigit = true;
                config.Password.RequireNonAlphanumeric = true;
                config.Password.RequireUppercase = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomUserClaimsPrincipalFactory>();

            //Add timespan of 2 hours for generated tokens
            services.Configure<DataProtectionTokenProviderOptions>(config =>
            config.TokenLifespan = TimeSpan.FromHours(2));

            //Creates Authorization Policies
            services.AddAuthorization(config =>
            {
                var authPolicyBuilder = new AuthorizationPolicyBuilder();
                //Fallback Policy only requires users be authenticated
                config.FallbackPolicy = authPolicyBuilder
                    .RequireAuthenticatedUser()
                    .Build();

                config.AddPolicy("Developer", policyBuilder =>
                {
                    policyBuilder.RequireRole("Developer");
                });

                config.AddPolicy("Admin", policyBuilder =>
                {
                    policyBuilder.RequireRole("Admin");
                });

                config.AddPolicy("Submitter", policyBuilder =>
                {
                    policyBuilder.RequireRole("Submitter");
                });

                config.AddPolicy("Manager", policyBuilder =>
                {
                    policyBuilder.RequireRole("Manager");
                });

                //config.AddPolicy("RequireNameScott", policyBuilder =>
                //{
                //    policyBuilder.RequireClaim(ClaimTypes.Name);
                //});

                //config.AddPolicy("RequireSpecificName", policyBuilder =>
                //{
                //    policyBuilder.AddRequirements(new NameRequirement("Scott"));
                //});
            });
            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
            services.AddScoped<IAuthorizationHandler, NameRequirementHandler>();

            //Creates an Identity Cookie for login/register
            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "Identity.Cookie";
                //Default path before authentication
                config.LoginPath = "/Account/Login";
            });

            //Allows updates at compile time
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

        //Creates and Adds roles to the Role Manager
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            string[] roleNames = { "Admin", "Manager", "Developer", "Submitter" };
            foreach (string roleName in roleNames)
            {
                bool roleExists = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    var role = new IdentityRole();
                    role.Name = roleName;
                    await RoleManager.CreateAsync(role);
                }
            }

            //Add Test users for each Role
            string[] userNames = { "DemoAdmin", "DemoManager", "DemoDeveloper", "DemoSubmitter" };
            foreach (string userName in userNames)
            {
                var user = await UserManager.FindByNameAsync(userName);
                if (user == null)
                {
                    var newUser = new ApplicationUser
                    {
                        UserName = userName
                    };
                    var result = await UserManager.CreateAsync(newUser, "DemoPassword");
                    if (result.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(newUser, userName.Substring(4));
                    }
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            //Creates Demo users for each Role
            CreateRoles(serviceProvider).Wait();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
