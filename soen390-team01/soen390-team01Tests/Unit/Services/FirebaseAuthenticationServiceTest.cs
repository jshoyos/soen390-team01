﻿using Firebase.Auth;
using Moq;
using NUnit.Framework;
using soen390_team01.Data.Exceptions;
using soen390_team01.Services;
using System;

namespace soen390_team01Tests.Unit.Services
{
    public class FirebaseAuthenticationServiceTest
    {
        Mock<IFirebaseAuthProvider> _authFirebaseProviderMock;

        [OneTimeSetUp]
        public void Setup()
        {
            _authFirebaseProviderMock = new Mock<IFirebaseAuthProvider>();
        }

        [Test]
        public void AuthenticateUserTest()
        {
            _authFirebaseProviderMock.Setup(a => a.SignInWithEmailAndPasswordAsync("existingEmail@hotmail.com", It.IsAny<string>())).Throws(new Exception("message\": \"EMAIL_NOT_FOUND\""));

            var authFirebaseService = new AuthenticationFirebaseService(_authFirebaseProviderMock.Object);
            Assert.Throws<EmailNotFoundException>(() => authFirebaseService.AuthenticateUser("existingEmail@hotmail.com", "badkfjdfks"));
        }
        [Test]
        public void UnexpectedExceptionTest()
        {
            _authFirebaseProviderMock.Setup(a => a.SignInWithEmailAndPasswordAsync("existingEmail@hotmail.com", It.IsAny<string>())).Throws(new Exception("message\": \"INVALID_PASSWORD\""));

            var authFirebaseService = new AuthenticationFirebaseService(_authFirebaseProviderMock.Object);
            Assert.Throws<UnexpectedDataAccessException>(() => authFirebaseService.AuthenticateUser("existingEmail@hotmail.com", "badkfjdfks"));
        }

        [Test]
        public void RegisterUserTest()
        {
            _authFirebaseProviderMock.Setup(a => a.CreateUserWithEmailAndPasswordAsync("existingEmail@hotmail.com", It.IsAny<string>(), "", false)).Throws(new Exception("message\": \"EMAIL_EXISTS\""));

            var authFirebaseService = new AuthenticationFirebaseService(_authFirebaseProviderMock.Object);
            Assert.Throws<EmailExistsException>(() => authFirebaseService.RegisterUser("existingEmail@hotmail.com", "badkfjdfks"));
        }

        [Test]
        public void RequestPasswordTest()
        {
            _authFirebaseProviderMock.Setup(a => a.SendPasswordResetEmailAsync("existingEmail@hotmail.com")).Throws(new Exception("message\": \"EMAIL_NOT_FOUND\""));

            var authFirebaseService = new AuthenticationFirebaseService(_authFirebaseProviderMock.Object);
            Assert.Throws<EmailNotFoundException>(() => authFirebaseService.RequestPasswordChange("existingEmail@hotmail.com"));
        }
        
    }
}
