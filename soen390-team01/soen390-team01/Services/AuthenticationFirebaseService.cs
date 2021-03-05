using Firebase.Auth;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using soen390_team01.Models;
using System.Text.RegularExpressions;
using soen390_team01.Data.Exceptions;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Diagnostics.CodeAnalysis;

namespace soen390_team01.Services
{
    public class AuthenticationFirebaseService : DisposableService
    {
        private readonly IFirebaseAuthProvider _ap;

        public AuthenticationFirebaseService()
        {
            _ap = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyD_HlK6kr9gptfYidc7_4Egn7uHwHes2pI"));
        }

        public AuthenticationFirebaseService(IFirebaseAuthProvider ap)
        {
            _ap = ap;
        }

        /// <summary>
        /// Method used to expose the firebase service of authentication through email and password
        /// </summary>
        /// <param name="email">user's email</param>
        /// <param name="password">user's password</param>
        /// <returns>boolean wether authentication was succesful or not</returns>
        public virtual async Task<bool> AuthenticateUser(string email, string password)
        {
            try
            {
                var auth = await _ap.SignInWithEmailAndPasswordAsync(email, password);
                return auth.User != null;
            }
            catch(Exception e)
            {
                FirebaseErrors(e.Message);
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
                FirebaseErrors(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Method to expose the firebase service to request a password change via email
        /// </summary>
        /// <param name="email">user's email</param>
        /// <returns></returns>
        public virtual async Task<bool> RequestPasswordChange(string email)
        {
            try
            {
                await _ap.SendPasswordResetEmailAsync(email);
                return true;
            }
            catch (Exception e)
            {
                FirebaseErrors(e.Message);
                return false;
            }
        }

        public static void FirebaseErrors(string error)
        {
            var temp = new Regex("message\": \".*\"").Matches(error)[0].Value.Substring(11).Replace('"', ' ').Trim();
            switch (temp)
            {
                case "EMAIL_EXISTS":
                    throw new EmailExistsException();
                case "EMAIL_NOT_FOUND":
                    throw new EmailNotFoundException();
                case "INVALID_PASSWORD":
                    break;
                default:
                    Debug.Fail("This should not happen");
                    break;
            }
        }

        /// <summary>
        /// Sets the authentication cookie so user is remembered in the browser
        /// </summary>
        /// <param name="email"></param>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        public virtual async Task SetAuthCookie(string email, string role, HttpContext context)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
            };

            await AuthenticationHttpContextExtensions.SignInAsync(context, CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        [ExcludeFromCodeCoverage]
        public virtual async Task RemoveAuthCookie(HttpContext context)
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(context, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
