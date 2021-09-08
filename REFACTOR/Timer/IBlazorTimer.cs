using System;
using System.Threading;

namespace KOMTEK.KundeInnsyn.Common
{
    public interface IBlazorTimer
    {
        string Id { get; set; }
        Timer Timer { get; set; }
        bool TimerSet { get; }

        event Action TimerElasped;
        event EventHandler TimerEventElapsed;

        void SetTimer(int interval = 10000, int? startDelay = null, bool repeat = true, string id = null, Action<int> callback = null);
        void StopTimer();
    }
}