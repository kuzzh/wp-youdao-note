

namespace WPYoudaoNoteApp.Converters
{
    using System;
    using System.Windows.Data;
    using YoudaoNoteDataAccess;

    public class NoteStatusConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var noteStatus = (NoteStatus)value;
            return noteStatus != NoteStatus.Normal ? "未同步" : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
