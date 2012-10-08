using System;

namespace TwitterSearch.Model.Messages
{
    public class TwitterStatusMessage
    {
        public int HourlyLimit { get; set; }
        public string RawSource { get; set; }
        public int RemainingHits { get; set; }
        public DateTime ResetTime { get; set; }
        public long ResetTimeInSeconds { get; set; }
    }
}
