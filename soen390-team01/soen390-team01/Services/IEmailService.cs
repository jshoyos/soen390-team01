﻿using soen390_team01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soen390_team01.Services
{
    public interface IEmailService
    {

        public void SendEmail(string text, Roles role);
    }
}
