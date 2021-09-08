using KOMTEK.KundeInnsyn.Common.Exceptions;
using System;
using System.Threading;

namespace KOMTEK.KundeInnsyn.Common
{
    public class BlazorTimer : IBlazorTimer
    {
        public Timer Timer { get; set; }
        public event Action TimerElasped;
        public event EventHandler TimerEventElapsed;
        private void AlertTimerElapsed() => TimerElasped?.Invoke();
        public bool TimerSet => Timer != null;
        public string Id { get; set; }

        protected virtual void OnTimerEventElapsed(EventArgs e)
        {
            TimerEventElapsed?.Invoke(this, e);
        }

        /// <summary>
        /// Sets the timer Object according to given parameters. 
        /// </summary>
        /// <param name="interval">Interval period between event triggers in milliseconds</param>
        /// <param name="startDelay">Delay until first trigger in milliseconds</param>
        /// <param name="repeat">If true, the timer will re-trigger every interval</param>
        /// <param name="id">Guid ID for this timer object</param>
        /// <param name="callback"></param>
        public void SetTimer(int interval = 10000, int? startDelay = null, bool repeat = true, string id = null, Action<int> callback = null)
        {
            if (TimerSet) throw new TimerAlreadySetException("Timer is already set. Run Dispose to avoid potential memory leaks before setting new timer object!");
            ValidateMethodInputs(interval, startDelay);
            Id = id;
            startDelay = startDelay is null ? interval : startDelay;
            Timer = new Timer(new TimerCallback(_ =>
            {
                callback?.Invoke(interval);
                AlertTimerElapsed();
                var args = new TimerEventArgs
                {
                    Threshold = interval,
                    Delay = (int)startDelay,
                    Repeat = repeat,
                    Callback = callback,
                    Id = id
                };
                OnTimerEventElapsed(args);
            }), null, (int)startDelay, interval);
        }

        public void StopTimer()
        {
            Timer?.Dispose();
            Timer = null;
        }

        private static void ValidateMethodInputs(int interval, int? startDelay)
        {
            if (interval <= 0) throw new NegativeTimerArgumentException("Start delay must be positive and not zero!");
            if (startDelay <= 0) throw new NegativeTimerArgumentException("Start delay must be positive and not zero!");
        }
    }
}
