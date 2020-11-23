namespace PCE_Web.Models
{
    public interface IEmailSenderInterface
    {
        void SendEmail(string code, string email);

        void AnswerReportMessage(string email, int identifier, string messageText = "");
    }
}