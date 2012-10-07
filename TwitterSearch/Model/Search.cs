using System;

namespace TwitterSearch.Model
{
    public class Search
    {
        public Search()
        {
        }

        public Search(string searchText, IDisposable searchObservable)
        {
            SearchText = searchText;
            SearchObservable = searchObservable;
        }

        public string SearchText { get; set; }
        public IDisposable SearchObservable { get; set; }
    }
}
