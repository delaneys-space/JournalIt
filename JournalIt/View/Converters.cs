using System;
using System.Globalization;


namespace JournalIt.View.Converters
{
    using System.Windows.Data;

    [ValueConversion(typeof(bool), typeof(double))]
    public class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, 
                              Type targetType, 
                              object parameter, 
                              CultureInfo culture)
        {
            var isOn = value != null && (bool)value;
            var fontSize = isOn ?
                Properties.UI.Default.StopwatchFontSizeSelected :
                Properties.UI.Default.StopwatchFontSize;

            return fontSize;
        }

        public object ConvertBack(object value, 
                                  Type targetType, 
                                  object parameter, 
                                  CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(string), typeof(System.Windows.Visibility))]
    public class StringToVisibiltyConverter : IValueConverter
    {
        public object Convert(object value, 
                              Type targetType, 
                              object parameter,
                              CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? 
                System.Windows.Visibility.Collapsed :
                System.Windows.Visibility.Visible;
        }

        public object ConvertBack(object value, 
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(string))]
    public class OnOffConverter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            if (value != null && (bool)value)
                return "ON";

            return "OFF";
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeConverter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            var s = "00:00:00";

            try
            {
                if (value != null)
                {
                    var timeSpan = new TimeSpan(0, 0, (int)value);
                    s = timeSpan.ToString().Substring(0, 8);
                }
            }
            catch
            {
                // ignored
            }

            return s;

        }
        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            var seconds = 0;
            try
            {
                var sTimeSpan = (string)value;
                TimeSpan.TryParse(sTimeSpan, out var timeSpan);

                seconds = (int)timeSpan.TotalSeconds;
            }
            catch
            {
                // ignored
            }

            return seconds;
        }
    }

    public class DateConverter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            var sDateTime = "01/01/2013 00:00:00";

            try
            {
                if (value != null)
                {
                    var dateTime = (DateTime)value;
                    sDateTime = dateTime.ToString(CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                // ignored
            }

            return sDateTime;

        }
        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            DateTime date = DateTime.MinValue;

            try
            {
                if (value != null)
                    date = (DateTime) value;
            }
            catch
            {
                // ignored
            }

            return date;
        }
    }

    public class NotConverter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            var bValue = false;
            if (value != null)
                bValue = (bool)value;

            return !bValue;
        }
        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
