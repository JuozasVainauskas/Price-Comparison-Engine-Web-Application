namespace PCE_Web.Models
{
    public interface IEmailSender
    {
        void SendEmail(string code, string email);

        void AnswerReportMessage(string email, int identifier, string messageText = "");
    }
}