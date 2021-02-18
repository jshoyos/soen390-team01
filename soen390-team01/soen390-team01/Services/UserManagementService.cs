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
        public virtual bool AddUser(User user)
        {
            using(var r = Rijndael.Create())
            {
                r.GenerateIV(); 
                _context.Users.Add(EncryptUser(user,r.IV));
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

        public virtual bool EditUser(User editedUser)
        {
            editedUser.Iv = _context.Users.Where(u => u.UserId == editedUser.UserId).Select(x => x.Iv).FirstOrDefault();

            _context.Users.Update(EncryptUser(editedUser));
            _context.SaveChanges();
            return true;
        }
        /// <summary>
        /// Retrieves all users from the User table
        /// </summary>
        /// <returns>List of all users</returns>
        public virtual List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            foreach(var user in _context.Users.ToList())
            {
                users.Add(DecryptUser(user));
            }
            return users;
        }

        public virtual User GetUserById(long id)
        {
            User user = _context.Users.Where(u => u.UserId == id).FirstOrDefault();
            return DecryptUser(user);
        }

        private User EncryptUser(User user, byte[] IV=null)
        {

            if (IV == null) {
                IV = Convert.FromBase64String(user.Iv);
            }
            return new User
            {
                FirstName = _encryption.Encrypt(user.FirstName, IV),
                LastName = _encryption.Encrypt(user.LastName, IV),
                Email = _encryption.Encrypt(user.Email, IV),
                PhoneNumber = _encryption.Encrypt(user.PhoneNumber, IV),
                Role = user.Role,
                Iv = Convert.ToBase64String(IV),
                UserId = user.UserId
            };
        }
        private User DecryptUser(User user)
        {
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