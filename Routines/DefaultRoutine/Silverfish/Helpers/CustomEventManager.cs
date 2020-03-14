using System;

namespace Silverfish.Routines.DefaultRoutine.Silverfish.Helpers
{
    public class MulliganStartedEventArgs
    {
        public bool ConcedeSuccessfully { get; set; }
    }

    public class CustomEventManager
    {
        private CustomEventManager()
        {

        }

        private static readonly CustomEventManager _instance = new CustomEventManager();

        public static CustomEventManager Instance
        {
            get { return _instance; }
        }

        public event EventHandler<MulliganStartedEventArgs> MulliganStarted;

        public bool OnMulliganStarted()
        {
            MulliganStartedEventArgs eventArgs = new MulliganStartedEventArgs();
            var temp = MulliganStarted;
            if (temp != null)
            {
                temp.Invoke(this, eventArgs);
            }
            return eventArgs.ConcedeSuccessfully;
        }
    }
}
