﻿using System;

namespace CustomDialogs.Converters
{
    internal class BoolToSelectionMode : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value as bool?) ?? false ? ListViewSelectionMode.Multiple : ListViewSelectionMode.Extended;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ((value as ListViewSelectionMode?) ?? ListViewSelectionMode.Extended) == ListViewSelectionMode.Multiple;
        }
    }
}