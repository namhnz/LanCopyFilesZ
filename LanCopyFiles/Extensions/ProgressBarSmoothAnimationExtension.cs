using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace LanCopyFiles.Extensions;

// Nguon: https://stackoverflow.com/a/14487836/7182661
public static class ProgressBarSmoothAnimationExtension
{
    private static TimeSpan duration = TimeSpan.FromSeconds(1);

    public static void SetPercent(this ProgressBar progressBar, double percentage)
    {
        DoubleAnimation animation = new DoubleAnimation(percentage, duration);
        progressBar.BeginAnimation(ProgressBar.ValueProperty, animation);
    }
}