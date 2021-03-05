using Firebase.Auth;
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
            //_authFirebaseProviderMock.Setup(a => a.SignInWithEmailAndPasswordAsync(It.Is<string>(s => !s.Equals("existingEmail@hotmail.com")), It.IsAny<string>())).ReturnsAsync(new FirebaseAuthLink(_authFirebaseProviderMock.Object, new FirebaseAuth()));

            var authFirebaseService = new AuthenticationFirebaseService(_authFirebaseProviderMock.Object);
            Assert.ThrowsAsync<EmailNotFoundException>(() => authFirebaseService.AuthenticateUser("existingEmail@hotmail.com", "badkfjdfks"));
            //Assert.IsTrue(authFirebaseService.AuthenticateUser("admin@hotmail.com", "bruu").Result);
        }

        [Test]
        public void RegisterUserTest()
        {
            _authFirebaseProviderMock.Setup(a => a.CreateUserWithEmailAndPasswordAsync("existingEmail@hotmail.com", It.IsAny<string>(), "", false)).Throws(new Exception("message\": \"EMAIL_EXISTS\""));

            var authFirebaseService = new AuthenticationFirebaseService(_authFirebaseProviderMock.Object);
            Assert.ThrowsAsync<EmailExistsException>(() => authFirebaseService.RegisterUser("existingEmail@hotmail.com", "badkfjdfks"));
        }

        [Test]
        public void RequestPasswordTest()
        {
            _authFirebaseProviderMock.Setup(a => a.SendPasswordResetEmailAsync("existingEmail@hotmail.com")).Throws(new Exception("message\": \"EMAIL_NOT_FOUND\""));

            var authFirebaseService = new AuthenticationFirebaseService(_authFirebaseProviderMock.Object);
            Assert.ThrowsAsync<EmailNotFoundException>(() => authFirebaseService.RequestPasswordChange("existingEmail@hotmail.com"));
        }
        
    }
}
