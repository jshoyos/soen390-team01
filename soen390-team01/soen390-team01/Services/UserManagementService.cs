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

        public UserManagementService(ErpDbContext context,EncryptionService encryption)
        {
            _context = context;
            _encryption = encryption;
        }

        /// <summary>
        /// Inserting encrypted user to the User table
        /// </summary>
        /// <param name="user"></param>
        public void AddUser(User user)
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
           
            _context.SaveChanges();
        }

        /// <summary>
        /// Retrieves all users from the User table
        /// </summary>
        /// <returns>List of all users</returns>
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }
    }
}
