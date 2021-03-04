using System;
using System.Collections.Generic;
using System.Text;

using HB.FullStack.XamarinForms;

using Xamarin.Forms;

namespace Demo
{
    public class DemoNavigationService : NavigationService
    {
        private readonly bool _needLoginBeforeAll;

        public DemoNavigationService(bool needLoginBeforeAll) : base(()=>Shell.Current.Navigation)
        {
            _needLoginBeforeAll = needLoginBeforeAll;
        }

        public override void PushLoginPage(bool animated = true)
        {
            
        }

        public override void ResumeRouting()
        {
             
        }
    }
}
