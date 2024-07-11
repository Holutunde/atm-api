
using Application.Common.ResultsModel;

namespace Application.Interfaces
{
      public interface IEmailSender
    {
        Task<Result> SendEmailAsync(string toEmail, string subject, string message);
    }
}