using System;

namespace PCE_Web.Controllers
{
    public class SendingInformation
    {
        public delegate void ButtonPushedEventHandler<in TArgs>(object source, TArgs args) where TArgs : EventArgs;
        public event ButtonPushedEventHandler<ServiceEventArgs> ButtonPushed;
        public void Pushed(string code, string email)
        {
            OnButtonPushed(code, email);
        }

        protected virtual void OnButtonPushed(string code, string email)
        {
            ButtonPushed?.Invoke(this, new ServiceEventArgs()
            {
                Code = code, 
                Email = email
            });
        }
    }
}