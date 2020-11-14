namespace PCE_Web.Controllers
{
    public class SendingInformation
    {
        public delegate void ButtonPushedEventHandler(object source, ServiceEventArgs args);
        public event ButtonPushedEventHandler ButtonPushed;
        public void Pushed(string code, string email)
        {
            OnButtonPushed(code, email);
        }

        protected virtual void OnButtonPushed(string code, string email)
        {
            if (ButtonPushed != null)
            {
                ButtonPushed(this, new ServiceEventArgs(){ Code = code, Email = email });
            }
        }
    }
}