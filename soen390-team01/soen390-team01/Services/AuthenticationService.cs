using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Services
{
    public class AuthenticationService
    {
        private readonly FirebaseAuthProvider _ap;

        public AuthenticationService()
        {
            _ap = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyD_HlK6kr9gptfYidc7_4Egn7uHwHes2pI"));
        }

        /// <summary>
        /// Method used to expose the firebase service of authentication through email and password
        /// </summary>
        /// <param name="email">user email</param>
        /// <param name="password">user password</param>
        /// <returns>boolean wether authentication was succesful or not</returns>
        public async Task<bool> AuthenticateUser(string email, string password)
        {
            try
            {
                var auth = await _ap.SignInWithEmailAndPasswordAsync(email, password);
                return auth.User != null;
            }
            catch(Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Method to expose the firebase service to create an account with email and password
        /// </summary>
        /// <param name="email">user email</param>
        /// <param name="password">user password</param>
        /// <returns>boolean wether the account was succesfully created</returns>
        public async Task<bool> RegisterUser(string email, string password)
        {
            try
            {
                await _ap.CreateUserWithEmailAndPasswordAsync(email, password);
                return true;
            }
            catch (Exception e)
            {
                Debug.Fail(e.Message);
                return false;
            }
        }

        public async Task RequestPasswordChange(string email)
        {
            await _ap.SendPasswordResetEmailAsync(email);
        }
    }
}
