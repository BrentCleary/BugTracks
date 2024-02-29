using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using BugTracks.Models;

namespace BugTracks.Services
{
    public class BTEmailService : IEmailSender
    {

        private readonly MailSettings _mailSettings;

        public BTEmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            
        }
    }
}
