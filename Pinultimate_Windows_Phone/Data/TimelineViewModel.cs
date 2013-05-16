using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pinultimate_Windows_Phone.Data
{
    public class TimelineViewModel
    {
        private MainPage mainPage { get; set; }
        private ProgressBar timeline
        {
            get
            {
                return mainPage.Timeline;
            }
            set
            {
                mainPage.Timeline = value;
            }
        }
        private TextBlock timeLabel
        {
            get
            {
                return mainPage.TimeLabel;
            }
            set
            {
                mainPage.TimeLabel = value;
            }
        }

        private ApplicationBarViewModel applicationBarViewModel
        {
            get
            {
                return mainPage.applicationBarViewModel;
            }
            set
            {
                mainPage.applicationBarViewModel = value;
            }
        }

        private TrendMapViewModel trendMapViewModel
        {
            get
            {
                return mainPage.trendMapViewModel;
            }
            set
            {
                mainPage.trendMapViewModel = value;
            }
        }

        public TimelineViewModel(MainPage MainPage)
        {
            mainPage = MainPage;
            timeLabel.Foreground = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
            timeline.Minimum = 0;
            timeline.Maximum = 24;
            timeline.Value = timeline.Maximum;
            timeLabel.Text = LabelText(GetCurrentTime());
            //applicationBarViewModel.ConfigureTimelineButtonsOnCondition();
            timeline.ValueChanged += Timeline_ValueChanged;

        }

        public void ChangeNumHours(int numHours)
        {
            timeline.Maximum = numHours;
            //ConfigureApplicationBarButtons();
            applicationBarViewModel.ConfigureTimelineButtonsOnCondition();
        }

        public int GetMaximum()
        {
            return (int)timeline.Maximum;
        }

        public int GetMinimum()
        {
            return (int)timeline.Minimum;
        }

        public int GetCurrentValue()
        {
            return (int)timeline.Value;
        }

        public DateTime GetMaximumTime()
        {
            DateTime current = DateTime.Now;
            return NormalizeDateTime(current);
        }

        public DateTime GetMinimumTime()
        {
            DateTime maximumTime = GetMaximumTime();
            return maximumTime.Subtract(TimeSpan.FromDays(1));
        }

        public DateTime GetCurrentTime()
        {
            DateTime maximumTime = GetMaximumTime();
            return maximumTime.Subtract(TimeSpan.FromHours(GetMaximum() - GetCurrentValue()));
        }

        private DateTime NormalizeDateTime(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
        }

        private String GetDateDescription(DateTime dt)
        {
            DateTime current = DateTime.Now;
            if (current.Date == dt.Date)
            {
                return "Today";
            }
            else if (current.Subtract(TimeSpan.FromDays(1)).Date == dt.Date)
            {
                return "Yesterday";
            } 
            else
            {
                return dt.ToString("ddd");
            }
        }

        private String GetTimeDescription(DateTime dt)
        {
            return dt.ToShortTimeString();
        }

        private void ConfigureApplicationBarButtons()
        {
            int value = (int)timeline.Value;
            if (value == timeline.Maximum)
            {
                applicationBarViewModel.DisableNextButton();
            }
            else if (value < timeline.Maximum)
            {
                applicationBarViewModel.EnableNextButton();
            }
            if (value == timeline.Minimum)
            {
                applicationBarViewModel.DisablePrevButton();
            }
            else if (value > timeline.Minimum)
            {
                applicationBarViewModel.EnablePrevButton();
            }
        }

        private void Timeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            timeLabel.Text = LabelText(GetCurrentTime());
            //ConfigureApplicationBarButtons();
            applicationBarViewModel.ConfigureTimelineButtonsOnCondition();
            trendMapViewModel.selectClustersForGivenTime();
        }

        private String LabelText(DateTime normalizedTime)
        {
            return GetDateDescription(normalizedTime) + " " + GetTimeDescription(normalizedTime);
        }

        public void IncreaseHour()
        {
            int value = (int)timeline.Value;
            if (value < timeline.Maximum)
            {
                timeline.Value = value + 1;
            }
        }

        public void DecreaseHour()
        {
            int value = (int)timeline.Value;
            if (value > timeline.Minimum)
            {
                timeline.Value = value - 1;
            }
        }

        public void setVisibility(Visibility visibility)
        {
            timeline.Visibility = visibility;
            timeLabel.Visibility = visibility;
        }

        internal void setOpacity(double opacity)
        {
            timeline.Opacity = opacity;
            timeLabel.Opacity = opacity;
        }
    }
}
