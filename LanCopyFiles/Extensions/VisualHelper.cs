﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace LanCopyFiles.Extensions;

public class VisualHelper
{
    public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj)
        where T : DependencyObject
    {
        if (depObj != null)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child != null && child is T)
                {
                    yield return (T)child;
                }

                foreach (T childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }
    }

    public static childItem FindVisualChild<childItem>(DependencyObject obj)
        where childItem : DependencyObject
    {
        foreach (childItem child in FindVisualChildren<childItem>(obj))
        {
            return child;
        }

        return null;
    }
}