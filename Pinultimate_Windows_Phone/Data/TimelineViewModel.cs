using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pinultimate_Windows_Phone.Data
{
    public class TimelineViewModel
    {
        public MainPage mainPage { get; set; }
        public ProgressBar timeline
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
        public TextBlock timeLabel
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

        public ApplicationBarViewModel applicationBarViewModel
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

        public TimelineViewModel(MainPage MainPage)
        {
            mainPage = MainPage;
            timeline.Minimum = 1;
            timeline.Maximum = 24;
            timeline.ValueChanged += Timeline_ValueChanged;
            timeline.Value = timeline.Maximum;

            timeLabel.Foreground = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
            timeLabel.Text = timeline.Value.ToString();
        }

        public void ChangeRange(int minimum, int maximum)
        {
            timeline.Minimum = minimum;
            timeline.Maximum = maximum;
            ConfigureApplicationBarButtons();
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
            timeLabel.Text = timeline.Value.ToString();
            ConfigureApplicationBarButtons();
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
    }
}
