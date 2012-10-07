using System;
using System.Reactive.Linq;
using TwitterSearch.Model;

namespace TwitterSearch.Design
{
    public class DesignTwitterFeed : TwitterFeedBase, ITwitterFeed
    {
        private readonly string[] _fakeScreenNames;
        private readonly Random _random;

        public DesignTwitterFeed()
        {
            _fakeScreenNames = new[] {"@mikkoj", "@justinbieber"};
            _random = new Random();
        }

        private Tweet MakeTwitterStatus(long tweetId)
        {
            var index = (int) (tweetId %_fakeScreenNames.Length);
            var fakeScreenName = _fakeScreenNames[index];
            return new Tweet
            {
                ProfileImageUrl = @"http://a0.twimg.com/sticky/default_profile_images/default_profile_5_normal.png",
                Id = tweetId,
                ScreenName = fakeScreenName,
                UserName = fakeScreenName,
                Text = "tweet " + tweetId
            };
        }

        public IObservable<Tweet> TweetsBySearch(string query)
        {
            return Observable.Generate<long, Tweet>(
                initialState: _random.Next(),
                condition: _ => true,
                iterate: _ => _random.Next(),
                resultSelector: MakeTwitterStatus,
                timeSelector: _ => TimeSpan.FromSeconds(0.3))
                .ReplayLastByKey(tws => tws.ScreenName)
                .Publish()
                .RefCount();
        }
    }
}

