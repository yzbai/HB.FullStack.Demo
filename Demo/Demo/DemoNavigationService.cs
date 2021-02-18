using System;
using System.Collections.Generic;
using System.Text;

using HB.FullStack.XamarinForms;

using Xamarin.Forms;

namespace Demo
{
    public class DemoNavigationService : NavigationService
    {
        public override void PopoutIntroduce()
        {
        }

        public override void PopoutLogin(bool animated = true)
        {
        }

        public override void PopoutRegisterProfilePage()
        {
        }

        public override void PushLoginPage(bool animated = true)
        {
        }

        public override void ResetMainPage()
        {
            Application.Current.MainPage = new AppShell();
        }

        internal static void Init()
        {
            DemoNavigationService demoNavigationService = new DemoNavigationService();

            Current = demoNavigationService;

            Current.ResetMainPage();
        }
    }
}
