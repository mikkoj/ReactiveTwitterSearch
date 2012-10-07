using System;

namespace TwitterSearch.Model
{
    public interface ITwitterFeed
    {
        IObservable<Tweet> TweetsBySearch(string query);
        int UserThrottleMs { get; set; }
    }
}