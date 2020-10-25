using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Email
{
    public interface IEmailSender
    {
        void SendEmail(EmailMessage message);
    }
}
