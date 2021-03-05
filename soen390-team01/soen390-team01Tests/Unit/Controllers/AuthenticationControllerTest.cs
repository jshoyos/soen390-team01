using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using soen390_team01.Controllers;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Models;
using soen390_team01.Services;
using System.Threading.Tasks;

namespace soen390_team01Tests.Unit.Controllers
{
    public class AuthenticationControllerTest
    {
        Mock<ErpDbContext> _contextMock;
        Mock<AuthenticationFirebaseService> _authenticationServiceMock;
        Mock<EncryptionService> _encryptionServiceMock;
        Mock<UserManagementService> _userManagementServiceMock;

        [OneTimeSetUp]
        public void Setup()
        {
            _contextMock = new Mock<ErpDbContext>();
            _authenticationServiceMock = new Mock<AuthenticationFirebaseService>();
            _encryptionServiceMock = new Mock<EncryptionService>("xg05/WzFW88jHrFxuNGy3vIMC8SMdFBTr/S2r+EPTtY=");
            _userManagementServiceMock = new Mock<UserManagementService>(_contextMock.Object, _encryptionServiceMock.Object);
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
        public  async Task IndexAsyncTest()
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
            _authenticationServiceMock.Setup(u => u.AuthenticateUser(It.Is<string>(s => !string.IsNullOrEmpty(s)), It.Is<string>(s => !string.IsNullOrEmpty(s)))).Returns(Task.FromResult(true));
            _authenticationServiceMock.Setup(c => c.SetAuthCookie(It.Is<string>(s => !string.IsNullOrEmpty(s)), It.Is<string>(s => !string.IsNullOrEmpty(s)), null)).Returns(Task.FromResult(true));

            _authenticationServiceMock.Setup(u => u.AuthenticateUser(It.Is<string>(s => string.IsNullOrEmpty(s)), It.Is<string>(s => string.IsNullOrEmpty(s)))).Returns(Task.FromResult(false));
            _authenticationServiceMock.Setup(c => c.SetAuthCookie(It.Is<string>(s => string.IsNullOrEmpty(s)), It.Is<string>(s => string.IsNullOrEmpty(s)), null)).Returns(Task.FromResult(false));

            var controller = new AuthenticationController(_authenticationServiceMock.Object, _userManagementServiceMock.Object);
            var result =  await controller.IndexAsync(model) as LocalRedirectResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Url.Equals("/Home/Privacy"));

            var invalidModel = new LoginModel
            {
                Email = "",
                Password = ""
            };
            var result2 = await controller.IndexAsync(invalidModel) as ViewResult;

            Assert.IsNotNull(result2);
            Assert.IsTrue((result2.Model as LoginModel).Email.Equals(invalidModel.Email));
        }

        [Test]
        public async Task ForgotPasswordTest()
        {
            var model = new LoginModel
            {
                Email = "admin@hotmail.com",
                Password = ""

            };
            _authenticationServiceMock.Setup(u => u.RequestPasswordChange(It.Is<string>(s => !string.IsNullOrEmpty(s)))).Returns(Task.FromResult(true));
            var controller = new AuthenticationController(_authenticationServiceMock.Object, _userManagementServiceMock.Object);
            var result = await controller.ForgotPassword(model) as LocalRedirectResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Url.Equals("/Authentication/Index"));

            var invalidModel = new LoginModel();
            var result2 = await controller.ForgotPassword(invalidModel) as ViewResult;

            Assert.IsNotNull(result2);
        }

        [Test]
        public async Task Logout()
        {
            _authenticationServiceMock.Setup(u => u.RemoveAuthCookie(new DefaultHttpContext()));
            var controller = new AuthenticationController(_authenticationServiceMock.Object, _userManagementServiceMock.Object);
            await controller.LogoutAsync();
        }
    }
}
