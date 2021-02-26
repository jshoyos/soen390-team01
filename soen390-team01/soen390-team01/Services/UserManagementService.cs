﻿using soen390_team01.Data;
using soen390_team01.Data.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using soen390_team01.Data.Exceptions;
using soen390_team01.Models;

namespace soen390_team01.Services
{
    public class UserManagementService : DisposableService
    {
        private readonly ErpDbContext _context;
        private readonly EncryptionService _encryption;

        public UserManagementService(ErpDbContext context, EncryptionService encryption)
        {
            _context = context;
            _encryption = encryption;
        }

        /// <summary>
        /// Inserting encrypted user to the User table
        /// </summary>
        /// <param name="user"></param>
        public virtual User AddUser(User user)
        {
            if(user.UserId != 0)
            {
                Debug.Fail("The id is autogenerated so no id should be given to the user");
                return null;
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
            catch (Exception e)
            {
                //TODO: catch the error and handle it
                return null;
            }
        }

        public virtual void RemoveUser(User user)
        {
            _context.Users.Remove(_context.Users.FirstOrDefault(u => u.Email.Equals(user.Email)));
            _context.SaveChanges();
        }
        /// <summary>
        /// Updates a user from the User table
        /// </summary>
        /// <param name="editedUser"></param>
        /// <returns></returns>
        public virtual User EditUser(User editedUser)
        {
            try
            {
                var user = _context.Users.Where(u => u.UserId == editedUser.UserId).FirstOrDefault();
                user.FirstName = editedUser.FirstName;
                user.LastName = editedUser.LastName;
                user.PhoneNumber = editedUser.PhoneNumber;
                user.Role = editedUser.Role;
                user.Email = editedUser.Email;

                _context.Users.Update(EncryptUser(user));
                _context.SaveChanges();
                return editedUser;
            }
            catch (Exception e)
            {
                // TODO: add exception handling to be able to create error messages
                return null;
            }
        }
        /// <summary>
        /// Retrieves all decrypted users from the User table
        /// </summary>
        /// <returns>List of all users</returns>
        public virtual List<User> GetAllUsers()
        {
            return _context.Users.ToList().Select(DecryptUser).ToList();
        }

        /// <summary>
        /// Retrieves a decrypted user from the user table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual User GetUserById(long id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            return DecryptUser(user);
        }

        private User EncryptUser(User user, byte[] iv=null)
        {
            iv ??= Convert.FromBase64String(user.Iv);
            user.FirstName = _encryption.Encrypt(user.FirstName, iv);
            user.LastName = _encryption.Encrypt(user.LastName, iv);
            user.Email = _encryption.Encrypt(user.Email, iv);
            user.PhoneNumber = _encryption.Encrypt(user.PhoneNumber, iv);
            user.Iv = Convert.ToBase64String(iv);

            return user;
        }
        private User DecryptUser(User user)
        {
            byte[] iv = Convert.FromBase64String(user.Iv);
            return new User
            {
                FirstName = _encryption.Decrypt(user.FirstName, iv),
                LastName = _encryption.Decrypt(user.LastName, iv),
                Email = _encryption.Decrypt(user.Email, iv),
                PhoneNumber = _encryption.Decrypt(user.PhoneNumber, iv),
                Role = user.Role,
                UserId = user.UserId,
                Iv = Convert.ToBase64String(iv)
            };
        }
    }
}