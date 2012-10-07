using System;
using System.Collections.ObjectModel;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using TwitterSearch.Model;


namespace TwitterSearch.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ITwitterFeed _twitterFeed;
        private string _searchQuery;
        private IDisposable _currentSearch;
        private int _throttleMs;

        public MainViewModel()
        {
        }

        [PreferredConstructor]
        public MainViewModel(ITwitterFeed twitterFeed)
        {
            _twitterFeed = twitterFeed;
            Tweets = new ObservableCollection<Tweet>();
            SubscribeToSearchCommand = new RelayCommand(SubscribeToNewSearch);
            ThrottleMs = 2000;
        }

        public string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                _searchQuery = value;
                RaisePropertyChanged("SearchQuery");
            }
        }

        public int ThrottleMs
        {
            get { return _throttleMs; }
            set
            {
                _throttleMs = value;
                _twitterFeed.UserThrottleMs = value;
                RaisePropertyChanged("ThrottleMs");
            }
        }

        public RelayCommand SubscribeToSearchCommand { get; private set; }

        private void SubscribeToNewSearch()
        {
            if (string.IsNullOrEmpty(SearchQuery))
            {
                return;
            }

            if (_currentSearch != null)
            {
                _currentSearch.Dispose();
            }
            var searchFeed = _twitterFeed.TweetsBySearch(SearchQuery);
            _currentSearch = HandleNewTweets(searchFeed);

        }

        private IDisposable HandleNewTweets(IObservable<Tweet> tweets)
        {
            return Tweets.MergeInsertAtTop(tweets, tweet => tweet.Id);
        }

        public ObservableCollection<Tweet> Tweets { get; set; }
    }
}