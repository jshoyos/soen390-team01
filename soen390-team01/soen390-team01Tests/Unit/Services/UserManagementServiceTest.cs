﻿using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Services;
using System.Linq;

namespace soen390_team01Tests.Unit.Services
{
    public class UserManagementServiceTest
    {
        private ErpDbContext _context;
        private UserManagementService _userManagementService;
        private EncryptionService _encryptionService;

        [OneTimeSetUp]
        public void Setup()
        {
            var builder = new DbContextOptionsBuilder<ErpDbContext>();
            builder.UseInMemoryDatabase("test_db");
            _context = new ErpDbContext(builder.Options);
            _encryptionService = new EncryptionService("xg05/WzFW88jHrFxuNGy3vIMC8SMdFBTr/S2r+EPTtY=");
            _userManagementService = new UserManagementService(_context, _encryptionService);
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
            Assert.IsNotNull(_userManagementService.AddUser(new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "admin@hotmail.com",
                PhoneNumber = "4385146677",
                Role = "admin"
            }));

            // Should be false because the email should be unique
            Assert.IsNull(_userManagementService.AddUser(new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "admin@hotmail.com",
                PhoneNumber = "4385146677",
                Role = "admin"
            }));

            Assert.AreEqual(1, _context.Users.ToList().Count);
        }

        [Test, Order(2)]
        public void GetAllUsersTest()
        {
            Assert.IsNotNull(_userManagementService.AddUser(new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "admin2@hotmail.com",
                PhoneNumber = "4385146677",
                Role = "admin"
            }));
            var users = _userManagementService.GetAllUsers();

            Assert.IsNotNull(users);
            Assert.IsNotEmpty(users);
            Assert.AreEqual(2, users.Count);

            // Checking that the decryption works
            Assert.IsTrue("admin".Equals(users[0].Role));
            Assert.IsTrue("John".Equals(users[0].FirstName));
            Assert.IsTrue("admin2@hotmail.com".Equals(users[1].Email));
        }

        [Test, Order(3)]
        public void GetUserByIdTest()
        {
            var userId = _context.Users.ToList()[1].UserId;
            var user = _userManagementService.GetUserById(userId);

            Assert.NotNull(user);
            Assert.IsTrue("admin2@hotmail.com".Equals(user.Email));
            Assert.AreEqual(userId, user.UserId);
        }

        [Test]
        public void EditUserTest()
        {
            var userToEdit = _context.Users.ToList().ElementAt(0);
            var newFirstName = "Editing The User";
            userToEdit.FirstName = newFirstName;

            Assert.IsNotNull(_userManagementService.EditUser(userToEdit));
            
            var editedUser = _userManagementService.GetUserById(_context.Users.ToList()[0].UserId);
            // Firstname update succeeded
            Assert.AreEqual(newFirstName, editedUser.FirstName);
        }

        [Test]
        public void RemoveUser()
        {
            var user = _context.Users.ToList().ElementAt(1);
            _userManagementService.RemoveUser(user);
            Assert.AreEqual(1, _context.Users.ToList().Count);
        }
    }
}
