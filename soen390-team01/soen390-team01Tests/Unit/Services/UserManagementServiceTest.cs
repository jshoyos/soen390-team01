using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Services;
using System.Linq;
using soen390_team01.Data.Exceptions;

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
        public void AddUserValidTest()
        {
            var addedUser = _userManagementService.AddUser(new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "admin@hotmail.com",
                PhoneNumber = "4385146677",
                Role = "admin"
            });

            Assert.AreEqual(1, _context.Users.ToList().Count);
            Assert.IsNotNull(addedUser);
            Assert.AreEqual("John", addedUser.FirstName);
        }

        [Test, Order(2)]
        public void AddUserInvalidTest()
        {
            Assert.Throws<NonUniqueValueException>(() => _userManagementService.AddUser(new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "admin@hotmail.com",
                PhoneNumber = "4385146677",
                Role = "admin"
            }));

            Assert.Throws<NullValueException>(() => _userManagementService.AddUser(new User
            {
                LastName = "Doe",
                Email = "different_admin@hotmail.com",
                PhoneNumber = "4385146677",
                Role = "admin"
            }));
        }

        [Test, Order(3)]
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

        [Test, Order(4)]
        public void GetUserByIdTest()
        {
            var userId = _context.Users.ToList()[1].UserId;
            var user = _userManagementService.GetUserById(userId);

            Assert.NotNull(user);
            Assert.IsTrue("admin2@hotmail.com".Equals(user.Email));
            Assert.AreEqual(userId, user.UserId);
        }

        [Test, Order(5)]
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

        [Test, Order(6)]
        public void RemoveUser()
        {
            var user = _context.Users.ToList().ElementAt(1);
            _userManagementService.RemoveUser(user);
            Assert.AreEqual(1, _context.Users.ToList().Count);
        }

        [Test, Order(7)]
        public void GetUserByEmailTest()
        {
            var userId = _context.Users.ToList()[0].UserId;
            var user = _userManagementService.GetUserById(userId);
            Assert.IsNotNull(user);

            var user2 = _userManagementService.GetUserByEmail(user.Email);

            Assert.AreEqual(user.UserId, user2.UserId);
            Assert.IsTrue(user.LastName.Equals(user2.LastName));

            var nullUser = _userManagementService.GetUserByEmail("jake");
            Assert.IsNull(nullUser);
        }
    }
}