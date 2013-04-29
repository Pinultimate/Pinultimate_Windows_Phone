using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
namespace Pinultimate_Windows_Phone.Data
{
    class TimeSliderViewModel
    {
        public Slider timeSlider { get; set; }
        public Button sliderControl { get; set; }

        public TimeSliderViewModel(Slider TimeSlider, Button SliderControl)
        {
            timeSlider = TimeSlider;
            sliderControl = SliderControl;
        }
    }
}
