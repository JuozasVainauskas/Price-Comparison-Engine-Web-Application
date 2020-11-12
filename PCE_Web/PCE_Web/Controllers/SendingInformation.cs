namespace PCE_Web.Controllers
{
    public class SendingInformation
    {
        public delegate void ButtonPushedEventHandler(object source, ServiceEventArgs args);
        public event ButtonPushedEventHandler ButtonPushed;
        public void Pushed(string code)
        {
            OnButtonPushed(code);
        }

        protected virtual void OnButtonPushed(string code)
        {
            if (ButtonPushed != null)
            {
                ButtonPushed(this, new ServiceEventArgs(){Code=code});
            }
        }
    }
}