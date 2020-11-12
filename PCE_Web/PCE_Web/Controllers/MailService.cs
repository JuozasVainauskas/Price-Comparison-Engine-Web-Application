using PCE_Web.Classes;

namespace PCE_Web.Controllers
{
    public class MailService
    {
        public void OnButtonPushed(object source, ServiceEventArgs e)
        {
            EmailSender.SendEmail(e.Code, e.Email);
        }
    }
}