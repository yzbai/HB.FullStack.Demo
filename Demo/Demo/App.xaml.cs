﻿
using HB.FullStack.Database;
using HB.FullStack.XamarinForms;
using HB.FullStack.XamarinForms.Base;
using HB.FullStack.XamarinForms.IdBarriers;

using Microsoft.Extensions.DependencyInjection;

using System.IO;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace Demo
{

    public partial class App : BaseApplication
    {
        private AppShell? _mainShell;

        private AppShell MainShell
        {
            get
            {
                if (_mainShell == null)
                {
                    _mainShell = new AppShell();
                }

                return _mainShell;
            }
            set { _mainShell = value; }
        }

        public override string InitAssetFileName { get; set; } = null;

        public App(IServiceCollection services)
        {
            InitializeServices(services);
            InitializeComponent();

            MainPage = new AppShell();

            NavigationService.Current.ResumeRouting();
        }

        protected override NavigationService GetNavigationServiceImpl()
        {
            return new DemoNavigationService(false);
        }

        protected override void RegisterServices(IServiceCollection services)
        {
            //System
            services
                .Configure<AppOptions>(Configuration.GetSection("AppOptions"))
                .AddIdGen(Configuration.GetSection("IdGen"))
                .AddSQLite(options =>
                {
                    string dbName = $"time.{Environment}.db";
                    options.CommonSettings.Version = 1;
                    options.CommonSettings.AutomaticCreateTable = true;

                    options.Connections.Add(new DatabaseConnectionSettings
                    {
                        DatabaseName = dbName,
                        IsMaster = true,
                        ConnectionString = $"Data Source={Path.Combine(FileSystem.AppDataDirectory, dbName)}"
                    });
                })
                .AddTCaptcha(Configuration[$"AppOptions:{nameof(AppOptions.AppIdOfTCaptchaForSms)}"])
                .AddApiClient(Configuration.GetSection("ApiClient"))
                .AddIdBarrier();
        }

        /// <summary>
        /// ConfigureServices
        /// </summary>
        /// <exception cref="DatabaseException"></exception>
        /// <exception cref="MobileException"></exception>
        protected override void ConfigureServices()
        {
            //IdGen
            DependencyService.Resolve<IIdBarrierService>().Initialize();

            //Database
            AddInitTask(DependencyService.Resolve<IDatabase>().InitializeAsync());
        }

        public override void OnOfflineDataUsed()
        {
        }

        
    }
}
