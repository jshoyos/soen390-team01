using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
            user.FirstName = _encryption.Encrypt(user.FirstName);
            user.LastName = _encryption.Encrypt(user.LastName);
            user.Email = _encryption.Encrypt(user.Email);
            user.PhoneNumber = _encryption.Encrypt(user.PhoneNumber);
            _context.Users.Add(user);
        }
    }
}
