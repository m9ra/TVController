using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVControler
{
    class PlayInfo
    {
        public readonly string PlayedFile;

        public readonly int ActualSecond;

        public readonly int TotalDuration;

        public readonly bool IsStopped;

        public readonly string CurrentTransportState;

        internal PlayInfo(string playedFile, int actualSecond, int totalDuration, string currentTransportState)
        {
            CurrentTransportState = currentTransportState;
            IsStopped= CurrentTransportState == "STOPPED";
            PlayedFile = IsStopped ? null : playedFile;
            ActualSecond = actualSecond;
            TotalDuration = totalDuration;
        }
    }
}
