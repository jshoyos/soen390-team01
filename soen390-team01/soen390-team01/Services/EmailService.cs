using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace soen390_team01.Services
{
    public static class EmailService
    {


        public static void sendEmail(String status,String text) {

            if (status == "completed")
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("soen390Project@gmail.com", "Soen390!"),
                    EnableSsl = true,
                };

                smtpClient.Send("soen390Project@gmail.com", "tigran.kar@hotmail.com", "Payment completed", text);
            }

        }



    }
}
