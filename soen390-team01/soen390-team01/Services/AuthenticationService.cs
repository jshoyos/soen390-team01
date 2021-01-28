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
        public async Task<bool> AuthenticateUser(string email, string password)
        {
            try
            {
                var auth = await _ap.SignInWithEmailAndPasswordAsync(email, password);
                if (auth.User != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                Debug.Assert(false, "fail on the authentication" + e.Message);
                return false;
            }

        }
    }
}
