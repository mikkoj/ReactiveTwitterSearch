using GalaSoft.MvvmLight.Messaging;
using System;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using TweetSharp;


namespace TwitterSearch.Model
{
    public class TwitterFeed : TwitterFeedBase, ITwitterFeed
    {
        /// <summary>
        /// Tweets recursively as an observable feed.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IObservable<Tweet> TweetsBySearch(string query)
        {
            var futureTweets = Observable.Create<Tweet>(
                observer =>
                {
                    var isRunning = true;
                    long? sinceId = null;

                    Action<Action<TimeSpan>> recursiveTweetProcessor = self =>
                    {
                        if (!isRunning) return;

                        Action<TwitterSearchResult, TwitterResponse> processNewTweets = (searchResult, response) =>
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                Messenger.Default.Send(new TwitterDelayMessage
                                {
                                    CompletedIn = searchResult.CompletedIn
                                });
                                sinceId = searchResult.Statuses.Select(status => (long?) status.Id).Max() ?? sinceId;
                                foreach (var tweet in searchResult.Statuses.OrderBy(s => s.CreatedDate))
                                {
                                    observer.OnNext(ParseTweet(tweet));
                                }
                            }
                            else
                            {
                                observer.OnError(response.InnerException);
                            }

                            if (isRunning)
                            {
                                self(GetSleepTime(response.RateLimitStatus));
                            }
                        };

                        try
                        {
                            if (sinceId.HasValue)
                            {
                                Service.SearchSince(sinceId.Value, query, processNewTweets);
                            }
                            else
                            {
                                Service.Search(query, processNewTweets);
                            }
                        }
                        catch (Exception e)
                        {
                            observer.OnError(e);
                        }
                    };

                    Scheduler.Schedule(GetSleepTime(), recursiveTweetProcessor);
                    return () => { isRunning = false; };
                });

            return futureTweets.ReplayLastByKey(tws => tws.ScreenName)
                               .Publish()
                               .RefCount();
        }

        private static Tweet ParseTweet(TwitterSearchStatus searchStatus)
        {
            return new Tweet
            {
                Id = searchStatus.Id,
                Text = searchStatus.Text,
                ProfileImageUrl = searchStatus.ProfileImageUrl,
                Time = searchStatus.CreatedDate,
                ScreenName = searchStatus.FromUserScreenName,
                UserName = searchStatus.FromUserName
            };
        }
    }
}