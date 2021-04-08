using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Models;
using soen390_team01.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace soen390_team01Tests.Unit.Services
{
    class EmailServiceTest
    {
        Mock<EmailClient> _emailClientMock;
        EncryptionService _encryption;
        Mock<ErpDbContext> _erpDbContextMock;

        [SetUp]
        public void Setup()
        {

            _emailClientMock = new Mock<EmailClient>();
            _encryption = new EncryptionService("xg05/WzFW88jHrFxuNGy3vIMC8SMdFBTr/S2r+EPTtY=");
            _erpDbContextMock = new Mock<ErpDbContext>();
        }

        [Test]
        public void SendEmailTest()
        {

            using var r = Rijndael.Create();
            r.GenerateIV();
            var iv = Convert.ToBase64String(r.IV);
            List<User> users = new List<User>() {
                new User()
                {
                                 
                    Iv = iv,
                    Role=Roles.InventoryManager.ToString(),
                    FirstName = _encryption.Encrypt("def", r.IV),
                    LastName = _encryption.Encrypt("def", r.IV),
                    Email = _encryption.Encrypt("def@def.com", r.IV),
                    PhoneNumber = _encryption.Encrypt("def", r.IV),

                }

            };

            _emailClientMock.Setup(c => c.Send(It.IsAny<MailMessage>()));
            _erpDbContextMock.Setup(c => c.Users).Returns(users.AsQueryable().BuildMockDbSet().Object);
            var emailService = new EmailService(_erpDbContextMock.Object, _encryption, _emailClientMock.Object);
            emailService.SendEmail("", Roles.InventoryManager);
            _emailClientMock.Verify(m => m.Send(It.IsAny<MailMessage>()), Times.Once);

            _emailClientMock.Setup(c => c.Send(It.IsAny<MailMessage>()));
            emailService.SendEmail("", Roles.Accountant);
            _emailClientMock.Verify(m => m.Send(It.IsAny<MailMessage>()), Times.Once);
        }



    }
}
