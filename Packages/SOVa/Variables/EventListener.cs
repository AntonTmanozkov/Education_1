using System;

namespace SOVa
{
    internal class EventListener<TDelegate> : IEventListener where TDelegate : Delegate
    {
        public TDelegate Listener { get; set; }
        public int Pryority { get; set; }
        public bool InvokeWithArgs { get; set; }

        Delegate IEventListener.Listener => Listener;

        public void Invoke(object arg = null)
        {
            if (InvokeWithArgs) 
            {
                Listener.DynamicInvoke(arg);
            }
            else 
            {
                Listener.DynamicInvoke();
            }
        }
    }

    internal interface IEventListener 
    {
        public int Pryority { get; }
        public bool InvokeWithArgs { get; }
        public void Invoke(object arg = null);
        public Delegate Listener { get; }
    }
}
