namespace TwitterSearch.Model.Messages
{
    public class RemoveSearchTextMessage
    {
        public RemoveSearchTextMessage(string searchText)
        {
            SearchTextToBeRemoved = searchText;
        }

        public string SearchTextToBeRemoved { get; set; }
    }
}