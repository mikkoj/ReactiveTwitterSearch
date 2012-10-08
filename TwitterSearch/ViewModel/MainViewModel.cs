using System;
using System.Linq;
using System.Collections.ObjectModel;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

using TwitterSearch.Model;
using TwitterSearch.Model.Messages;


namespace TwitterSearch.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ITwitterFeed _twitterFeed;
        private string _searchQuery;
        private const int DefaultThrottleMs = 5000;
        private int? _throttleMs;
        private double _twitterDelayMs;

        public MainViewModel()
        {
        }

        [PreferredConstructor]
        public MainViewModel(ITwitterFeed twitterFeed)
        {
            _twitterFeed = twitterFeed;
            _twitterFeed.UserThrottleMs = DefaultThrottleMs;

            Tweets = new ObservableCollection<Tweet>();
            Searches = new ObservableCollection<SearchViewModel>();
            SubscribeToSearchCommand = new RelayCommand(SubscribeToNewSearch);
            Messenger.Default.Register<TwitterDelayMessage>(this, m =>
            {
                TwitterDelayMs = m.CompletedIn * 1000;
            });

            Messenger.Default.Register<RemoveSearchTextMessage>(this, m => RemoveSearch(m.SearchTextToBeRemoved));
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
            get { return _throttleMs ?? DefaultThrottleMs; }
            set
            {
                _throttleMs = value;
                _twitterFeed.UserThrottleMs = value;
                RaisePropertyChanged("ThrottleMs");
            }
        }

        public double TwitterDelayMs
        {
            get { return _twitterDelayMs; }
            set
            {
                _twitterDelayMs = value;
                RaisePropertyChanged("TwitterDelayMs");
            }
        }


        public RelayCommand SubscribeToSearchCommand { get; private set; }

        private void SubscribeToNewSearch()
        {
            if (string.IsNullOrEmpty(SearchQuery) ||
                Searches.Count >= 4)
            {
                return;
            }

            var searchFeed = _twitterFeed.TweetsBySearch(SearchQuery);
            var searchObservable = HandleNewTweets(searchFeed);
            Searches.Add(new SearchViewModel(SearchQuery, searchObservable));
            SearchQuery = "";
        }

        private void RemoveSearch(string searchText)
        {
            var existingSearch = Searches.FirstOrDefault(s => s.SearchText == searchText);
            if (existingSearch != null)
            {
                existingSearch.SearchObservable.Dispose();
                Searches.Remove(existingSearch);
                if (Searches.Count == 0)
                {
                    Tweets.Clear();
                }
            }
        }

        private IDisposable HandleNewTweets(IObservable<Tweet> tweets)
        {
            return Tweets.MergeInsertAtTop(tweets, tweet => tweet.Id);
        }

        public ObservableCollection<Tweet> Tweets { get; set; }
        public ObservableCollection<SearchViewModel> Searches { get; set; }
    }
}