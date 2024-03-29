﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using soen390_team01.Data;
using soen390_team01.Data.Entities;
using soen390_team01.Models;

namespace soen390_team01.Services
{
    public class EmailService : IEmailService
    {
        private readonly ErpDbContext _erpDbContext;
        private readonly EncryptionService _encryption;
        private readonly EmailClient _emailClient;
        

       
        public EmailService(ErpDbContext erpDbContext, EncryptionService encryption, EmailClient emailClient)
        {
            _erpDbContext = erpDbContext;
            _encryption = encryption;
            _emailClient = emailClient;
        }

        private List<User> GetUsers(Roles role)
        {
            return _erpDbContext.Users.Where(u => u.Role == role.ToString()).ToList().ConvertAll(DecryptUser);
        }

        private User DecryptUser(User user)
        {
            var iv = Convert.FromBase64String(user.Iv);
            return new User
            {
                FirstName = _encryption.Decrypt(user.FirstName, iv),
                LastName = _encryption.Decrypt(user.LastName, iv),
                Email = _encryption.Decrypt(user.Email, iv),
                PhoneNumber = _encryption.Decrypt(user.PhoneNumber, iv),
                Role = user.Role,
                UserId = user.UserId,
                Added = user.Added,
                Updated = user.Updated,
                Iv = Convert.ToBase64String(iv)
            };
        }

        public void SendEmail(string text, Roles role)
        {
            List<User> users = GetUsers(role);
            if (users.Count <= 0)
            {
                return;
            }
               
            MailAddressCollection mailAddresses = new MailAddressCollection();

            MailMessage message = new MailMessage("soen390Project@gmail.com", string.Join(",", users.Select(u => u.Email).ToList()));
            message.Subject = "ERP Update";
            message.Body = text;
            _emailClient.Send(message);

        }

    }
}
