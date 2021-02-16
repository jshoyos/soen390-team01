using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace soen390_team01.Services
{
    public class UserManagementService
    {
        private readonly ErpDbContext _context;
        private readonly EncryptionService _encryption;
        private readonly AuthenticationFirebaseService _authenticationFirebaseService;

        public UserManagementService(ErpDbContext context,EncryptionService encryption, AuthenticationFirebaseService authenticationFirebaseService)
        {
            _context = context;
            _encryption = encryption;
            _authenticationFirebaseService = authenticationFirebaseService;
        }

        /// <summary>
        /// Inserting encrypted user to the User table
        /// </summary>
        /// <param name="user"></param>
        public bool AddUser(User user)
        {
            using(var r = Rijndael.Create())
            {
                r.GenerateIV(); 
                _context.Users.Add(new User
                {
                    FirstName = _encryption.Encrypt(user.FirstName, r.IV),
                    LastName = _encryption.Encrypt(user.LastName, r.IV),
                    Email = _encryption.Encrypt(user.Email, r.IV),
                    PhoneNumber = _encryption.Encrypt(user.PhoneNumber, r.IV),
                    Role = user.Role,
                    Iv = Convert.ToBase64String(r.IV)
                });
            }
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                //TODO: catch the error and handle it
                return false;
            }
        }

        /// <summary>
        /// Retrieves all users from the User table
        /// </summary>
        /// <returns>List of all users</returns>
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            foreach(var user in _context.Users.ToList())
            {
                byte[] IV = Convert.FromBase64String(user.Iv);
                users.Add(new User
                {
                    FirstName = _encryption.Decrypt(user.FirstName, IV),
                    LastName = _encryption.Decrypt(user.LastName, IV),
                    Email = _encryption.Decrypt(user.Email, IV),
                    PhoneNumber = _encryption.Decrypt(user.PhoneNumber, IV),
                    Role = user.Role,
                    Iv = Convert.ToBase64String(IV)
                });
            }
            return users;
        }

        public User GetUserById(long id)
        {
            User user = _context.Users.Where(u => u.UserId == 68).FirstOrDefault();
            byte[] IV = Convert.FromBase64String(user.Iv);
            return new User
            {
                FirstName = _encryption.Decrypt(user.FirstName, IV),
                LastName = _encryption.Decrypt(user.LastName, IV),
                Email = _encryption.Decrypt(user.Email, IV),
                PhoneNumber = _encryption.Decrypt(user.PhoneNumber, IV),
                Role = user.Role,
                UserId = user.UserId,
                Iv = Convert.ToBase64String(IV)
            };
        }
    }
}