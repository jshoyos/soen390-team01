using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using soen390_team01.Models;
using soen390_team01.Services;
using System.Collections.Generic;
using System.Security.Claims;

namespace soen390_team01Tests.Unit.Data.Queries
{
    public class ModulePermissionAttributeTest
    {
        ClaimsIdentity claimsIdentity;
        List<Claim> claims;
        Mock<HttpContext> httpContextMock;

        [OneTimeSetUp]
        public void Setup()
        {
            claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, Role.Accountant)
            };
            claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.User).Returns(new ClaimsPrincipal(claimsIdentity));
        }
        [Test]
        public void AllowedModulePermissionTest()
        {
            var actionArguments = new Dictionary<string, object>();
            var attribute = new ModulePermissionAttribute
            {
                Roles = Role.Accountant
            };

            var context = new ActionExecutingContext(
                new ActionContext(
                    httpContextMock.Object,
                    Mock.Of<RouteData>(),
                    Mock.Of<ActionDescriptor>(),
                    new ModelStateDictionary()
                    ),
                new List<IFilterMetadata>(),
                actionArguments,
                Mock.Of<Controller>()
                );
            attribute.OnActionExecuting(context);
        }

        [Test]
        public void RefusedModulePermissionTest()
        {
            var actionArguments = new Dictionary<string, object>();
            var attribute = new ModulePermissionAttribute
            {
                Roles = Role.SalesRep
            };
            var context = new ActionExecutingContext(
                new ActionContext(
                    httpContextMock.Object,
                    new RouteData(
                        new RouteValueDictionary(
                        new Dictionary<string, object>()
                        {
                            {"action","Index" }
                        })
                    ),
                    Mock.Of<ActionDescriptor>(),
                    new ModelStateDictionary()
                    ),
                new List<IFilterMetadata>(),
                actionArguments,
                Mock.Of<Controller>()
                );
            attribute.OnActionExecuting(context);
        }
    }
}
