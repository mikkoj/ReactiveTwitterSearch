using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

using TwitterSearch.Model.Messages;


namespace TwitterSearch.Model
{
    public class SearchViewModel
    {
        public SearchViewModel()
        {
        }

        public SearchViewModel(string searchText, IDisposable searchObservable)
        {
            SearchText = searchText;
            SearchObservable = searchObservable;
            RemoveSearchCommand = new RelayCommand<string>(RemoveSearch);
        }


        public void RemoveSearch(string searchText)
        {
            Messenger.Default.Send(new RemoveSearchTextMessage(searchText));
        }

        public string SearchText { get; set; }
        public IDisposable SearchObservable { get; set; }
        public RelayCommand<string> RemoveSearchCommand { get; set; }
    }
}
