using System.Threading.Tasks;

namespace BookManagement.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
