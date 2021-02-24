using soen390_team01.Controllers;
using Moq;
using System.Collections.Generic;
using soen390_team01.Services;
using NUnit.Framework;
using soen390_team01.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data;
using soen390_team01.Models;

namespace soen390_team01Tests.Unit.Controllers
{
    public class UserManagementControllerTest
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
            var users = new List<User>
            {
                new User
                {
                    FirstName = "Juan",
                    LastName = "Se",
                    Email = "admin@hotmail.com",
                    PhoneNumber = "4385146677",
                    Role = "admin",
                    Iv = "FSedff453",
                    UserId = 1
                }
            };
            // GetAllUsers should return the expected list
            _userManagementServiceMock.Setup(u => u.GetAllUsers()).Returns(users);

            var controller = new UserManagementController(_authenticationServiceMock.Object, _userManagementServiceMock.Object);
            var result = controller.UserManagement() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.Model as UserManagementModel).Users.Count);

            users.Add(
                new User
                {
                    FirstName = "Juan",
                    LastName = "Se",
                    Email = "admin@hotmail.com",
                    PhoneNumber = "4385146677",
                    Role = "admin",
                    Iv = "FSedff453",
                    UserId = 2
                }
            );

            result = controller.UserManagement() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(2, (result.Model as UserManagementModel).Users.Count);
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
                Role = "admin",
                Iv = "FSedff453",
                UserId = 2
            });

            var controller = new UserManagementController(_authenticationServiceMock.Object, _userManagementServiceMock.Object);
            var result = controller.GetUserById(5) as PartialViewResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull((result.Model as User));
        }
    }
}
