using System;
using soen390_team01.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using soen390_team01.Data;
using soen390_team01.Data.Exceptions;
using soen390_team01.Services;

namespace soen390_team01.Models
{ 
    public class UserManagementModel : FilteredModel, IUserManagementService
    {
        #region fields
        private readonly ErpDbContext _context;
        private readonly EncryptionService _encryption;
        #endregion

        #region properties
        public AddUserModel AddUserModel { get; set; }
        public EditUserModel EditUserModel { get; set; }
        public List<User> Users { get; set; }
        #endregion
        public UserManagementModel() { }

        /// <summary>
        /// User Management Constructor taking the database context and the encryption service
        /// </summary>
        /// <param name="context"></param>
        /// <param name="encryption"></param>
        public UserManagementModel(ErpDbContext context, EncryptionService encryption)
        {
            _context = context;
            _encryption = encryption;
            Users = GetAllUsers();
        }

        #region methods
        /// <summary>
        /// Gets all the users in the database and empties the addusermodel as well as the edit user model
        /// </summary>
        public void Reset()
        {
            Users = GetAllUsers();
            AddUserModel = new AddUserModel();
            EditUserModel = new EditUserModel();
        }

        /// <summary>
        /// Inserting encrypted user to the User table
        /// </summary>
        /// <param name="user"></param>
        public User AddUser(User user)
        {
            if (user.UserId != 0)
            {
                throw new UnauthorizedInsertionException("User");
            }

            if (_context.Users.Any() && _context.Users.Select(u => _encryption.Decrypt(u.Email, Convert.FromBase64String(u.Iv))).ToList().Contains(user.Email))
            {
                throw new NonUniqueValueException("Email");
            }

            try
            {
                using var r = Rijndael.Create();
                r.GenerateIV();

                var addedUser = _context.Users.Add(new User(EncryptUser(user, r.IV))).Entity;
                _context.SaveChanges();

                return DecryptUser(addedUser);
            }
            catch (DbUpdateException e)
            {
                throw DbAccessExceptionProvider.Provide(e.InnerException as PostgresException);
            }
            catch (ArgumentNullException)
            {
                throw new NullValueException();
            }
        }

        /// <summary>
        /// Removes a specific user from the database
        /// </summary>
        /// <param name="user"></param>
        public void RemoveUser(User user)
        {
            _context.Users.Remove(_context.Users.FirstOrDefault(u => u.Email.Equals(user.Email))!);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates a user from the User table
        /// </summary>
        /// <param name="editedUser"></param>
        /// <returns></returns>
        public User EditUser(User editedUser)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == editedUser.UserId);
                user.FirstName = editedUser.FirstName;
                user.LastName = editedUser.LastName;
                user.PhoneNumber = editedUser.PhoneNumber;
                user.Role = editedUser.Role;
                user.Email = editedUser.Email;

                _context.Users.Update(EncryptUser(user));
                _context.SaveChanges();
                return DecryptUser(user);
            }
            catch (DbUpdateException e)
            {
                throw DbAccessExceptionProvider.Provide(e.InnerException as PostgresException);
            }
        }

        /// <summary>
        ///     Retrieves all decrypted users from the User table
        /// </summary>
        /// <returns>List of all users</returns>
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList().ConvertAll(DecryptUser);
        }

        /// <summary>
        ///     Retrieves a decrypted user from the user table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetUserById(long id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == id);
                return DecryptUser(user);
            }
            catch (Exception)
            {
                throw new NotFoundException("User", "ID", id.ToString());
            }
        }

        /// <summary>
        /// Gets an user from the database with a specific email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetUserByEmail(string email)
        {
            try
            {
                List<User> users = GetAllUsers();
                return users.Find(u => u.Email == email);
            }
            catch (Exception)
            {
                throw new NotFoundException("User", "email", email);
            }
        }

        // Used to encrypt all the data that can be linked to an user (i.e email, name..)
        // This method is used when adding or updating an user into the database
        private User EncryptUser(User user, byte[] iv = null)
        {
            iv ??= Convert.FromBase64String(user.Iv);
            user.FirstName = _encryption.Encrypt(user.FirstName, iv);
            user.LastName = _encryption.Encrypt(user.LastName, iv);
            user.Email = _encryption.Encrypt(user.Email, iv);
            user.PhoneNumber = _encryption.Encrypt(user.PhoneNumber, iv);
            user.Iv = Convert.ToBase64String(iv);

            return user;
        }

        // Used to decrypt all the encrypted data of an user
        // This method is used when getting an user from the database
        private User DecryptUser(User user)
        {
            var iv = Convert.FromBase64String(user.Iv);
            return new User
            {
                FirstName = _encryption.Decrypt(user.FirstName, iv),
                LastName = _encryption.Decrypt(user.LastName, iv),
                Email = _encryption.Decrypt(user.Email, iv),
                PhoneNumber = _encryption.Decrypt(user.PhoneNumber, iv),
                Role = user.Role,
                UserId = user.UserId,
                Added = user.Added,
                Updated = user.Updated,
                Iv = Convert.ToBase64String(iv)
            };
        }
        #endregion
    }
}
