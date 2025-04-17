using Syncfusion.Maui.Chat.Helpers;
using Syncfusion.Maui.Core;
using Syncfusion.Maui.DataSource.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssistViewMAUI
{
    public class NewChatIconColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if((bool)value)
            {
                return Color.FromArgb("#6E6E6E");
            }
            else
            {
                return Color.FromArgb("#C8C8C8");
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ClearIconVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var text = value as string;
            if (text != null && !string.IsNullOrEmpty(text))
            {
                return true;
            }

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return false;
        }
    }

    public class SearchIconConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return "\ue70e";
            }

            return "\ue728";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SendRequestStateConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var text = value as string;
            var sendIcon = parameter as SfChip;
            if ((text != null && string.IsNullOrWhiteSpace(text)) || text == null)
            {
                if (sendIcon != null)
                {
                    sendIcon.TextColor = Color.FromArgb("#ACACAC");
                    sendIcon.Background = Color.FromArgb("#E1E1E1");
                }
                return false;
            }

            sendIcon.Background = Color.FromArgb("#6750A4");
            sendIcon.TextColor = Color.FromArgb("#FFFFFF");
            return true;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return true;
        }
    }

    public class EditorExpandIconVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {           
            if (value != null && (double)value >= 70)
            {
                return true;
            }

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return false;
        }
    }

    public class TemporaryChatVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var name = (parameter as Label).Text;
            if ((bool)value && name != null && name == "Temporary Chat")
            {
                return true;
            }

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return false;
        }
    }

    public class CustomGroupComparer : IComparer<GroupResult>
    {
        public int Compare(GroupResult? x, GroupResult? y)
        {
            var xenum = (GroupName)x!.Key;
            var yenum = (GroupName)y!.Key;

            if (xenum.CompareTo(yenum) == -1)
            {
                return -1;
            }
            else if (xenum.CompareTo(yenum) == 1)
            {
                return 1;
            }

            return 0;
        }
    }
    public class GroupHeaderTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            switch ((GroupName)value!)
            {
                case GroupName.Today:
                    return "Today";
                case GroupName.Yesterday:
                    return "Yesterday";
                case GroupName.Previous7Days:
                    return "Previous 7 Days";
                case GroupName.LastMonth:
                    return "Previous 30 Days";
                case GroupName.Older:
                    return "Older";
                default:
                    return "";
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
