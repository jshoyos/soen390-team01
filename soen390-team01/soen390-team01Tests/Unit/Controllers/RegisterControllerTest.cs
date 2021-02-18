using soen390_team01.Controllers;
using Moq;
using System.Collections.Generic;
using soen390_team01.Services;
using NUnit.Framework;
using soen390_team01.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using soen390_team01.Data;

namespace soen390_team01Tests.Unit.Controllers
{
    public class RegisterControllerTest
    {
        Mock<ErpDbContext> contextMock;
        Mock<AuthenticationFirebaseService> authenticationServiceMock;
        Mock<EncryptionService> encryptionServiceMock;
        Mock<UserManagementService> userManagementServiceMock;

        [SetUp]
        public void Setup()
        {
            contextMock = new Mock<ErpDbContext>();
            authenticationServiceMock = new Mock<AuthenticationFirebaseService>();
            encryptionServiceMock = new Mock<EncryptionService>("xg05/WzFW88jHrFxuNGy3vIMC8SMdFBTr/S2r+EPTtY=");
            userManagementServiceMock = new Mock<UserManagementService>(contextMock.Object, encryptionServiceMock.Object, authenticationServiceMock.Object);
        }

        [Test]
        public void IndexTest()
        {

            List<User> users = new List<User>()
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
            userManagementServiceMock.Setup(u => u.GetAllUsers()).Returns(users);

            RegisterController controller = new RegisterController(authenticationServiceMock.Object, userManagementServiceMock.Object);
            var result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, (result.ViewData["users"] as List<User>).Count);

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

            Assert.AreEqual(2, (result.ViewData["users"] as List<User>).Count);
        }

        [Test]
        public void GetUserByIdTest()
        {
            userManagementServiceMock.Setup(u => u.GetUserById(It.Is<long>(l => l>0))).Returns(new User
            {
                FirstName = "Juan",
                LastName = "Se",
                Email = "admin@hotmail.com",
                PhoneNumber = "4385146677",
                Role = "admin",
                Iv = "FSedff453",
                UserId = 2
            });

            RegisterController controller = new RegisterController(authenticationServiceMock.Object, userManagementServiceMock.Object);
            var result = controller.GetUserById(5) as PartialViewResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull((result.Model as User));
        }
    }
}
