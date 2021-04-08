using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace soen390_team01.Services
{
    public class EmailClient
    {
        public virtual void Send(MailMessage mailMessage)
        {
            new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("soen390Project@gmail.com", "Soen390!"),
                EnableSsl = true,
            }.Send(mailMessage);
        }
    }
}
