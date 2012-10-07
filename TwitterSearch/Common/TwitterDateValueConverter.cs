using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace TwitterSearch.Common
{
    public class TwitterDateValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var twitterCreatedDate = (DateTime) value;

            TimeSpan span = DateTime.UtcNow - twitterCreatedDate;
            var agoString = new StringBuilder();
            if (span.Days > 365)
            {
                agoString.AppendFormat("{0} year(s) ", span.Days/365);
            }
            if (span.Days % 365 > 30)
            {
                agoString.AppendFormat("{0} months ", (span.Days % 365) / 30);
            }
            if (span.Days % 365 % 30 > 7)
            {
                agoString.AppendFormat("{0} weeks ", span.Days % 365 % 30 / 7);
            }
            if (span.Days % 365 % 30 % 7 > 0)
            {
                agoString.AppendFormat("{0} days ", span.Days % 365 % 30 % 7);
            }
            if (span.Hours > 1)
            {
                agoString.AppendFormat("{0} hours ", span.Hours);
            }
            else if (span.Hours == 1)
            {
                agoString.AppendFormat("{0} hour ", span.Hours);
            }
            if (span.Minutes > 1)
            {
                agoString.AppendFormat("{0} minutes ", span.Minutes);
            }
            else if (span.Minutes == 1)
            {
                agoString.AppendFormat("{0} minute ", span.Minutes);
            }
            if (span.Seconds > 1)
            {
                agoString.AppendFormat("{0} seconds ", span.Seconds);
            }
            else if (span.Seconds == 1)
            {
                agoString.AppendFormat("{0} second ", span.Seconds);
            }
            
            return agoString + "ago";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}