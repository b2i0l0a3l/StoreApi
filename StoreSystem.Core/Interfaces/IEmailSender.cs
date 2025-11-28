using System.Threading.Tasks;

namespace StoreSystem.Core.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
    }
}
