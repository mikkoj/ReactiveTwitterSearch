using System;

namespace TwitterSearch.Model
{
    public class Tweet
    {
        public string Text { get; set; }
        public long Id { get; set; }
        public string ProfileImageUrl { get; set; }
        public string ScreenName { get; set; }
        public string UserName { get; set; }
        public DateTime Time { get; set; }
    }
}
