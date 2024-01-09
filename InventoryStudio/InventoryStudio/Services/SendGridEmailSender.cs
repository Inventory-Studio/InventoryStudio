using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid.Helpers.Mail;
using SendGrid;
using SendGrid.Helpers.Errors.Model;
using Microsoft.Extensions.Options;

namespace InventoryStudio.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly string _sendGridApiKey;
        private readonly ILogger<SendGridEmailSender> _logger;


        public SendGridEmailSender(IConfiguration configuration, ILogger<SendGridEmailSender> logger)
        {
            _sendGridApiKey = configuration["SendGrid:ApiKey"];
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                if (string.IsNullOrEmpty(_sendGridApiKey))
                {
                    throw new Exception("Null SendGridKey");
                }

                var client = new SendGridClient(_sendGridApiKey);
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress("no-reply@inventorystudio.com", "Inventory Studio"),
                    Subject = subject,
                    PlainTextContent = htmlMessage,
                    HtmlContent = htmlMessage
                };
                msg.AddTo(new EmailAddress(email));

                // Disable click tracking.
                // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
                msg.SetClickTracking(false, false);
                var response = await client.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email to {Email} was sent successfully.", email);
                }
                else
                {
                    _logger.LogWarning("Email to {Email} failed to send. Status: {StatusCode}", email, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "A network error occurred while sending email");
            }
            catch (Exception ex)
            {
                // 处理其他异常
                _logger.LogError(ex, "An unexpected error occurred while sending email");
            }
        }
    }


}