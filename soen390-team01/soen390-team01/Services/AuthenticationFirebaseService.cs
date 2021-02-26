using Firebase.Auth;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace soen390_team01.Services
{
    public class AuthenticationFirebaseService : IDisposable
    {
        private readonly FirebaseAuthProvider _ap;
        private bool _disposed;

        public AuthenticationFirebaseService()
        {
            _ap = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyD_HlK6kr9gptfYidc7_4Egn7uHwHes2pI"));
        }

        /// <summary>
        /// Method used to expose the firebase service of authentication through email and password
        /// </summary>
        /// <param name="email">user's email</param>
        /// <param name="password">user's password</param>
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
        /// <param name="email">user's email</param>
        /// <param name="password">user's password</param>
        /// <returns>boolean wether the account was succesfully created</returns>
        public virtual async Task<bool> RegisterUser(string email, string password)
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

        /// <summary>
        /// Method to expose the firebase service to request a password change via email
        /// </summary>
        /// <param name="email">user's email</param>
        /// <returns></returns>
        public async Task<bool> RequestPasswordChange(string email)
        {
            try
            {
                await _ap.SendPasswordResetEmailAsync(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
        }
    }
}
