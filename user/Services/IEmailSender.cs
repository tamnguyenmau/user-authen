using System.Threading.Tasks;

namespace user.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string body);

    }
}