using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace LanCopyFiles.Extensions;

// Nguon: https://stackoverflow.com/a/14487836/7182661
public static class ProgressBarSmoothAnimationExtension
{
    public static void SetPercent(this ProgressBar progressBar, double percentage)
    {
        if (percentage > 0)
        {
            DoubleAnimation animation = new DoubleAnimation(percentage, TimeSpan.FromSeconds(1));
            progressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
        }
        else
        {
            DoubleAnimation animation = new DoubleAnimation(percentage, TimeSpan.FromMilliseconds(1));
            progressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
        }
    }
}