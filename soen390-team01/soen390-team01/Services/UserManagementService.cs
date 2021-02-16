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

        public void AddUser(User user)
        {
            using(Rijndael r = Rijndael.Create())
            {
                r.GenerateIV();
                user.FirstName = _encryption.Encrypt(user.FirstName,r.IV);
                user.LastName = _encryption.Encrypt(user.LastName, r.IV);
                user.Email = _encryption.Encrypt(user.Email, r.IV);
                user.PhoneNumber = _encryption.Encrypt(user.PhoneNumber, r.IV);
                user.Iv = Convert.ToBase64String(r.IV);
            }
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }
    }
}
