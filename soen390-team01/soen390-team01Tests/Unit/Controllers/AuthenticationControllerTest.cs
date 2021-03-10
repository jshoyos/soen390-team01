using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using soen390_team01.Controllers;
using soen390_team01.Data.Entities;
using soen390_team01.Data.Exceptions;
using soen390_team01.Models;
using soen390_team01.Services;

namespace soen390_team01Tests.Unit.Controllers
{
    public class AuthenticationControllerTest
    {
        Mock<AuthenticationFirebaseService> _authenticationServiceMock;
        Mock<IUserManagementService> _userManagementServiceMock;

        [OneTimeSetUp]
        public void Setup()
        {
            _authenticationServiceMock = new Mock<AuthenticationFirebaseService>();
            _userManagementServiceMock = new Mock<IUserManagementService>();
        }

        [Test]
        public void IndexTest()
        {
            var controller = new AuthenticationController(_authenticationServiceMock.Object, _userManagementServiceMock.Object);
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public void PermissionDeniedTest()
        {
            var controller = new AuthenticationController(_authenticationServiceMock.Object, _userManagementServiceMock.Object);
            var result = controller.PermissionDenied() as ViewResult;
            Assert.IsNotNull(result);
        }

        [Test]
        public void IndexAsyncTest()
        {
            var user = new User
            {
                FirstName = "Juan",
                LastName = "Se",
                Email = "admin@hotmail.com",
                PhoneNumber = "4385146677",
                Role = "admin",
                Iv = "FSedff453",
                UserId = 1
            };
            var model = new LoginModel
            {
                Email = "admin@hotmail.com",
                Password = "admin1"

            };

            _userManagementServiceMock.Setup(u => u.GetUserByEmail(It.IsAny<string>())).Returns(user);
            _authenticationServiceMock.Setup(u => u.AuthenticateUser(It.Is<string>(s => !string.IsNullOrEmpty(s)), It.Is<string>(s => !string.IsNullOrEmpty(s)))).Returns(true);
            _authenticationServiceMock.Setup(c => c.SetAuthCookie(It.Is<string>(s => !string.IsNullOrEmpty(s)), It.Is<string>(s => !string.IsNullOrEmpty(s)), null));

            var controller = new AuthenticationController(_authenticationServiceMock.Object, _userManagementServiceMock.Object);
            var result =  controller.IndexAsync(model) as LocalRedirectResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Url.Equals("/Home/Privacy"));

            var invalidModel = new LoginModel
            {
                Email = "",
                Password = ""
            };
            var result2 = controller.IndexAsync(invalidModel) as ViewResult;

            Assert.IsNotNull(result2);
            Assert.IsTrue((result2.Model as LoginModel).Email.Equals(invalidModel.Email));
        }

        [Test]
        public void IndexAsyncExceptionTest()
        {
            _userManagementServiceMock.Setup(u => u.GetUserByEmail(It.IsAny<string>())).Throws(new InvalidValueException("test","Exception"));
            var model = new LoginModel
            {
                Email = "admin@hotmail.com",
                Password = "admin1"

            };

            var controller = new AuthenticationController(_authenticationServiceMock.Object, _userManagementServiceMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };

            var result = controller.IndexAsync(model) as ViewResult;

            Assert.IsTrue("User does not exist".Equals(result.TempData["errorMessage"]));
        }

        [Test]
        public void ForgotPasswordTest()
        {
            var model = new LoginModel
            {
                Email = "admin@hotmail.com",
                Password = ""

            };
            _authenticationServiceMock.Setup(u => u.RequestPasswordChange(It.Is<string>(s => !string.IsNullOrEmpty(s)))).Returns(true);
            var controller = new AuthenticationController(_authenticationServiceMock.Object, _userManagementServiceMock.Object);
            var result = controller.ForgotPassword(model) as LocalRedirectResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Url.Equals("/Authentication/Index"));

            var invalidModel = new LoginModel();
            var result2 = controller.ForgotPassword(invalidModel) as ViewResult;

            Assert.IsNotNull(result2);
        }

        [Test]
        public void ForgotPasswordViewTest()
        {
            var controller = new AuthenticationController(_authenticationServiceMock.Object, _userManagementServiceMock.Object);
            var result = controller.ForgotPassword() as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Logout()
        {
            _authenticationServiceMock.Setup(u => u.RemoveAuthCookie(new DefaultHttpContext()));
            var controller = new AuthenticationController(_authenticationServiceMock.Object, _userManagementServiceMock.Object);
            controller.LogoutAsync();
        }
    }
}

