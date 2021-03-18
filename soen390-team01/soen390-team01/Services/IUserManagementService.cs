using System.Collections.Generic;
using soen390_team01.Data.Entities;
using soen390_team01.Models;

namespace soen390_team01.Services
{
    public interface IUserManagementService : IFilteredModel
    {
        #region properties
        public AddUserModel AddUserModel { get; set; }
        public EditUserModel EditUserModel { get; set; }
        public List<User> Users { get; set; }
        #endregion

        #region methods
        public void Reset();

        /// <summary>
        /// Inserting encrypted user to the User table
        /// </summary>
        /// <param name="user"></param>
        public User AddUser(User user);

        public void RemoveUser(User user);

        /// <summary>
        /// Updates a user from the User table
        /// </summary>
        /// <param name="editedUser"></param>
        /// <returns></returns>
        public User EditUser(User editedUser);

        /// <summary>
        /// Retrieves all decrypted users from the User table
        /// </summary>
        /// <returns>List of all users</returns>
        public List<User> GetAllUsers();

        /// <summary>
        /// Retrieves a decrypted user from the user table by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetUserById(long id);

        /// <summary>
        /// Retrieves a decrypted user from the user table by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public User GetUserByEmail(string email);
        #endregion
    }
}
