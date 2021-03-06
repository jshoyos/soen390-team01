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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            var attribute = new ModulePermissionAttribute
            {
                Roles = Role.Accountant
            };

            var context = new ActionExecutedContext(
                new ActionContext(
                    httpContextMock.Object,
                    Mock.Of<RouteData>(),
                    Mock.Of<ActionDescriptor>(),
                    new ModelStateDictionary()
                    ),
                new List<IFilterMetadata>(),
                Mock.Of<Controller>()
                );
            attribute.OnActionExecuted(context);
        }

        [Test]
        public void RefusedModulePermissionTest()
        {
            var attribute = new ModulePermissionAttribute
            {
                Roles = Role.SalesRep
            };
            var context = new ActionExecutedContext(
                new ActionContext(
                    httpContextMock.Object,
                    Mock.Of<RouteData>(),
                    Mock.Of<ActionDescriptor>(),
                    new ModelStateDictionary()
                    ),
                new List<IFilterMetadata>(),
                Mock.Of<Controller>()
                );
            attribute.OnActionExecuted(context);
        }
    }
}
