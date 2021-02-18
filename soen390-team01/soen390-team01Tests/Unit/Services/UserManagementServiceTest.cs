using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace soen390_team01Tests.Unit.Services
{
    public class UserManagementServiceTest
    {
        private ErpDbContext _context;
        private UserManagementService _userManagementService;

        [OneTimeSetUp]
        public void Setup()
        {
            var builder = new DbContextOptionsBuilder<ErpDbContext>();
            builder.UseInMemoryDatabase("test_db");
            _context = new ErpDbContext(builder.Options);
            var authenticationServiceMock = new Mock<AuthenticationFirebaseService>();
            ///key used for testing: xg05/WzFW88jHrFxuNGy3vIMC8SMdFBTr/S2r+EPTtY=
            _userManagementService = new UserManagementService(_context, new EncryptionService("xg05/WzFW88jHrFxuNGy3vIMC8SMdFBTr/S2r+EPTtY="), authenticationServiceMock.Object);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            foreach (var entity in _context.Users)
            {
                _context.Users.Remove(entity);
            }
            _context.SaveChanges();
        }

        [Test, Order(1)]
        public void AddUsersTest()
        {
            Assert.IsTrue(_userManagementService.AddUser(new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "admin@hotmail.com",
                PhoneNumber = "4385146677",
                Role = "admin",
                UserId = 1
            }));

            // Should be false because the Userid is already used
            Assert.IsFalse(_userManagementService.AddUser(new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "admin3@hotmail.com",
                PhoneNumber = "4385146677",
                Role = "admin",
                UserId = 1
            }));

            // Should be false because the email should be unique
            Assert.IsFalse(_userManagementService.AddUser(new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "admin@hotmail.com",
                PhoneNumber = "4385146677",
                Role = "admin",
                UserId = 3
            }));

            Assert.AreEqual(1, _context.Users.ToList().Count);
        }

        [Test]
        public void GetAllUsersTest()
        {
            Assert.IsTrue(_userManagementService.AddUser(new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "admin2@hotmail.com",
                PhoneNumber = "4385146677",
                Role = "admin",
                UserId = 6
            }));
            var users = _userManagementService.GetAllUsers();

            Assert.IsNotNull(users);
            Assert.IsNotEmpty(users);
            Assert.AreEqual(1, users.Count);

            // Checking that the decryption works
            Assert.IsTrue("John".Equals(users[0].Role));
            Assert.IsTrue("John".Equals(users[0].FirstName));
            Assert.IsTrue("admin2@hotmail.com".Equals(users[1].Email));
        }
    }
}
