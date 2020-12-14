using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTracker.AuthorizationRequirements
{
    public class NameRequirement : IAuthorizationRequirement
    {
        public NameRequirement(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public class NameRequirementHandler : AuthorizationHandler<NameRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NameRequirement requirement)
        {
            var firstName = context.User.FindFirst("FirstName").Value;
            if (firstName == requirement.Name)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
