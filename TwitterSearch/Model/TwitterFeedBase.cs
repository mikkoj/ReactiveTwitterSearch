using GalaSoft.MvvmLight.Messaging;
using System;
using System.Reactive.Concurrency;
using TweetSharp;

namespace TwitterSearch.Model
{
    public abstract class TwitterFeedBase
    {
        protected readonly TwitterService Service;
        protected readonly IScheduler Scheduler;
        public int UserThrottleMs { get; set; }

        protected TwitterFeedBase()
        {
            UserThrottleMs = 0;
            Scheduler = TaskPoolScheduler.Default;
            Service = GetTwitterService();
        }

        private static TwitterService GetTwitterService()
        {
            return new TwitterService();
        }

        protected TimeSpan GetSleepTime()
        {
            if (Service.Response != null)
            {
                return GetSleepTime(Service.Response.RateLimitStatus);
            }
            return GetSleepTime(new TwitterRateLimitStatus
            {
                HourlyLimit = -1
            });
        }

        protected TimeSpan GetSleepTime(TwitterRateLimitStatus rateLimitStatus)
        {
            if (rateLimitStatus.HourlyLimit == -1)
            {
                return TimeSpan.FromMilliseconds(UserThrottleMs);
            }

            // let's send a message if the limit is not infinite
            Messenger.Default.Send(new TwitterStatusMessage
            {
                HourlyLimit = rateLimitStatus.HourlyLimit,
                RawSource = rateLimitStatus.RawSource,
                RemainingHits = rateLimitStatus.RemainingHits,
                ResetTime = rateLimitStatus.ResetTime,
                ResetTimeInSeconds = rateLimitStatus.ResetTimeInSeconds
            });

            var timeTillReset = rateLimitStatus.ResetTime - Scheduler.Now;
            var remainingHits = rateLimitStatus.RemainingHits == 0 ? 1 : rateLimitStatus.RemainingHits;
            var secondsOfSleepTime = timeTillReset.TotalSeconds / remainingHits;
            Console.WriteLine("sleep time: " + secondsOfSleepTime);
            return TimeSpan.FromSeconds(secondsOfSleepTime).Add(TimeSpan.FromMilliseconds(UserThrottleMs));
        }
    }
}