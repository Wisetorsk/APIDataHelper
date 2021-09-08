using System;

namespace KOMTEK.KundeInnsyn.Common
{
    public class TimerEventArgs : EventArgs
    {
        public string Id { get; set; }
        public int Threshold { get; set; }
        public int Delay { get; set; }
        public bool Repeat { get; set; }
        public Action<int> Callback { get; set; }
        public DateTime TimeReached { get; set; }
    }
}
