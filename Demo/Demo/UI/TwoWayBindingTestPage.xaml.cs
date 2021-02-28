using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TwoWayBindingTestPage : ContentPage
    {
        private string? _data;
        private BindableThing? _bindableThing;

        public string? Data
        {
            get => _data; 
            set
            {
                _data = value; 
                OnPropertyChanged();
            }
        }

        public TwoWayBindingTestPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Data = SecurityUtil.CreateRandomString(20);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _bindableThing = new BindableThing();

            _bindableThing.SetBinding(BindableThing.BDataProperty, new Binding(nameof(Data), source: this));
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            _bindableThing.ChangeBData();
        }
    }

    public class BindableThing : BindableObject
    {
        public static BindableProperty BDataProperty = BindableProperty.Create(nameof(BData), typeof(string), typeof(BindableThing), null, BindingMode.OneWayToSource, propertyChanged: (b, o, n) => ((BindableThing)b).OnBDataChanged((string?)o, (string?)n));

        private void OnBDataChanged(string? o, string? n)
        {
            //BData = SecurityUtil.CreateRandomNumbericString(10);
        }

        internal void ChangeBData()
        {
            BData = SecurityUtil.CreateRandomNumbericString(10);
        }

        public string? BData { get => (string?)GetValue(BDataProperty); set => SetValue(BDataProperty, value); }
    }
}