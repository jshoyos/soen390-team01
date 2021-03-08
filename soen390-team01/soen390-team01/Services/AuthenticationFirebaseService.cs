using Firebase.Auth;
using System;
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
        public virtual bool AuthenticateUser(string email, string password)
        {
            try
            {
                var auth = _ap.SignInWithEmailAndPasswordAsync(email, password).Result;
                return auth?.User != null;
            }
            catch(Exception e)
            {
                throw FirebaseErrors(e.Message);
            }

        }

        /// <summary>
        /// Method to expose the firebase service to create an account with email and password
        /// </summary>
        /// <param name="email">user's email</param>
        /// <param name="password">user's password</param>
        /// <returns>boolean wether the account was succesfully created</returns>
        public virtual bool RegisterUser(string email, string password)
        {
            try
            {
                _ap.CreateUserWithEmailAndPasswordAsync(email, password);
                return true;
            }
            catch (Exception e)
            {
                throw FirebaseErrors(e.Message);
            }
        }

        /// <summary>
        /// Method to expose the firebase service to request a password change via email
        /// </summary>
        /// <param name="email">user's email</param>
        /// <returns></returns>
        public virtual bool RequestPasswordChange(string email)
        {
            try
            {
                _ap.SendPasswordResetEmailAsync(email);
                return true;
            }
            catch (Exception e)
            {
                throw FirebaseErrors(e.Message);
            }
        }

        public static DataAccessException FirebaseErrors(string error)
        {
            var temp = new Regex("message\": \".*\"").Matches(error)[0].Value[11..].Replace('"', ' ').Trim();
            return temp switch
            {
                "EMAIL_EXISTS" => new EmailExistsException(),
                "EMAIL_NOT_FOUND" => new EmailNotFoundException(),
                _ => new UnexpectedDataAccessException("Email_error")
            };
        }

        /// <summary>
        /// Sets the authentication cookie so user is remembered in the browser
        /// </summary>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <param name="context"></param>
        [ExcludeFromCodeCoverage]
        public virtual void SetAuthCookie(string email, string role, HttpContext context)
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

            context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        [ExcludeFromCodeCoverage]
        public virtual void RemoveAuthCookie(HttpContext context)
        {
            context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
