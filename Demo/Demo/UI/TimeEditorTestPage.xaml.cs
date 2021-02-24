using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using HB.FullStack.Common;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TimeEditorTestPage : ContentPage
    {
        private Time24Hour _time;
        public Time24Hour Time { get => _time; set { _time = value; OnPropertyChanged(nameof(Time)); } }

        private bool _isDisplay24HourFormat;

        public bool IsDisplay24HourFormat { get => _isDisplay24HourFormat; set { _isDisplay24HourFormat = value; OnPropertyChanged(nameof(IsDisplay24HourFormat)); } }

        public ICommand RandomTimeCommand { get; set; }

        public TimeEditorTestPage()
        {
            InitializeComponent();

            Time = Time24Hour.LocalNow;

            RandomTimeCommand = new Command(OnRandomTime);

            BindingContext = this;
        }

        private void OnRandomTime()
        {
            Random random = new Random();
            Time = new Time24Hour(random.Next(1, 24), random.Next(0, 60));
            IsDisplay24HourFormat = random.Next() % 2 == 0;
        }
    }
}