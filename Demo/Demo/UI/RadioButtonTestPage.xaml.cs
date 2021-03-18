using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{

    public enum TestEnum
    {
        A,
        B
    }

    public static class TestEnumStatic
    {
        public static TestEnum A { get; set; } = TestEnum.A;
        public static TestEnum B { get; set; } = TestEnum.B;
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RadioButtonTestPage : ContentPage
    {
        private TestEnum? _selection;
        public TestEnum? Selection { get => _selection; set { _selection = value; OnPropertyChanged(); } }

        public RadioButtonTestPage()
        {
            InitializeComponent();

            Selection = TestEnum.B;

            BindingContext = this;

        }

    }
}