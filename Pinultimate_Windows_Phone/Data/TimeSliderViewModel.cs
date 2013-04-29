using System.Windows;
using System.Windows.Controls;
namespace Pinultimate_Windows_Phone.Data
{
    public class TimeSliderViewModel
    {
        public TrendMapViewModel trendMapViewModel { get; set; }
        public Slider timeSlider { get; set; }
        public Button sliderControl { get; set; }

        public TimeSliderViewModel(TrendMapViewModel TrendMapViewModel, Slider TimeSlider, Button SliderControl)
        {
            trendMapViewModel = TrendMapViewModel;
            timeSlider = TimeSlider;
            timeSlider.Value = TimeSlider.Maximum;
            timeSlider.ValueChanged += TimeSlider_ValueChanged;
            sliderControl = SliderControl;
            sliderControl.Click += SliderControl_OnClick;
        }


        private void TimeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int oldHourAgo = (int)e.OldValue;
            int newHourAgo = (int)e.NewValue;
        }

        private void SliderControl_OnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
