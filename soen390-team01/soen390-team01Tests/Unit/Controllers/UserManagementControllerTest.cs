using soen390_team01.Controllers;
using Moq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using soen390_team01.Services;
using NUnit.Framework;
using soen390_team01.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using soen390_team01.Models;
using soen390_team01.Data.Exceptions;
using Microsoft.Extensions.Logging;

namespace soen390_team01Tests.Unit.Controllers
{
    public class UserManagementControllerTest
    {
        Mock<AuthenticationFirebaseService> _authenticationServiceMock;
        Mock<IUserManagementService> _userManagementServiceMock;
        Mock<ILogger<UserManagementController>> _loggerMock;

        [OneTimeSetUp]
        public void Setup()
        {
            _authenticationServiceMock = new Mock<AuthenticationFirebaseService>();
            _userManagementServiceMock = new Mock<IUserManagementService>();
            _loggerMock = new Mock<ILogger<UserManagementController>>();
        }

        [Test]
        public void IndexTest()
        {
            var users = new List<User>
            {
                new User
                {
                    FirstName = "Juan",
                    LastName = "Se",
                    Email = "admin@hotmail.com",
                    PhoneNumber = "4385146677",
                    Role = "Admin",
                    Iv = "FSedff453",
                    UserId = 1
                }
            };
            // GetAllUsers should return the expected list
            _userManagementServiceMock.Setup(u => u.Reset());
            _userManagementServiceMock.Setup(u => u.Users).Returns(users);

            var controller = new UserManagementController(_authenticationServiceMock.Object, _userManagementServiceMock.Object, _loggerMock.Object);
            var result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as IUserManagementService).Users.Count);

            users.Add(
                new User
                {
                    FirstName = "Juan",
                    LastName = "Se",
                    Email = "admin@hotmail.com",
                    PhoneNumber = "4385146677",
                    Role = "Admin",
                    Iv = "FSedff453",
                    UserId = 2
                }
            );

            result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(2, ((IUserManagementService) result.Model).Users.Count);
        }

        [Test]
        public void GetUserByIdTest()
        {
            _userManagementServiceMock.Setup(u => u.GetUserById(It.Is<long>(l => l>0))).Returns(new User
            {
                FirstName = "Juan",
                LastName = "Se",
                Email = "admin@hotmail.com",
                PhoneNumber = "4385146677",
                Role = "Admin",
                Iv = "FSedff453",
                UserId = 5
            });
            _userManagementServiceMock.Setup(u => u.GetUserById(It.Is<long>(l => l < -5))).Throws(new NotFoundException("user","id","-1"));
            _userManagementServiceMock.Setup(u => u.GetUserById(It.Is<long>(l => l < 0 && l > -5))).Returns<User>(null);

            var controller = new UserManagementController(_authenticationServiceMock.Object, _userManagementServiceMock.Object, _loggerMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
            var result = controller.GetUserById(5) as PartialViewResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull((result.Model as User));

            var indexResult = controller.GetUserById(-1) as ViewResult;
            Assert.IsNotNull(indexResult);
            Assert.IsNotNull(indexResult.Model as IUserManagementService);
            Assert.IsNotNull(controller.GetUserById(-6));
        }

        [Test]
        public void AddUserTest()
        {
            _authenticationServiceMock.Setup(a => a.RegisterUser(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            _userManagementServiceMock.Setup(u => u.AddUser(It.IsAny<User>())).Returns(new User());

            var model = new Mock<UserManagementModel>();
            model.Object.AddUserModel = new AddUserModel();

            var controller = new UserManagementController(_authenticationServiceMock.Object, _userManagementServiceMock.Object, _loggerMock.Object) {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };

            Assert.IsNotNull(controller.AddUser(model.Object));
            Assert.AreEqual("Account registration failed: Try again later.", controller.TempData["errorMessage"]);
        }

        [Test]
        public void AddUserFailTest()
        {
            _authenticationServiceMock.Setup(a => a.RegisterUser(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _userManagementServiceMock.Setup(u => u.AddUser(It.IsAny<User>())).Returns(new User());
            _userManagementServiceMock.Setup(u => u.RemoveUser(It.IsAny<User>()));

            var model = new Mock<UserManagementModel>();
            model.Object.AddUserModel = new AddUserModel();
            var controller = new UserManagementController(_authenticationServiceMock.Object, _userManagementServiceMock.Object, _loggerMock.Object) {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };

            Assert.IsNotNull(controller.AddUser(model.Object));
        }

        [Test]
        public void EditUserTest()
        {
            _userManagementServiceMock.Setup(u => u.EditUser(It.IsAny<User>())).Returns(new User());

            var editUser = new EditUserModel(new User
            {
                FirstName = "Juan",
                LastName = "Se",
                Email = "admin@hotmail.com",
                PhoneNumber = "4385146677",
                Role = Roles.Admin.ToString(),
                Iv = "FSedff453",
                UserId = 5
            });

            var controller = new UserManagementController(_authenticationServiceMock.Object, _userManagementServiceMock.Object, _loggerMock.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };

            Assert.IsNotNull(controller.EditUser(editUser));
        }
    }
}
