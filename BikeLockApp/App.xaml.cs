using System;
using System.Threading.Tasks;
using BikeLockApp.Services;
using BikeLockApp.ViewModels;
using BikeLockApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BikeLockApp
{
    public partial class App : Application
    {
        ISettingsService _settingsService;
        public App()
        {
            InitializeComponent(); 
            ServiceContainer.Register<ISettingsService>(() => new SettingsService());
            _settingsService = ServiceContainer.Resolve<ISettingsService>();
            ServiceContainer.Register<INavigationService>(() => new NavigationService(_settingsService));

            ServiceContainer.Register<PairViewModel>(() => new PairViewModel());
            ServiceContainer.Register<HomeViewModel>(() => new HomeViewModel());
            ServiceContainer.Register<LoginViewModel>(() => new LoginViewModel());
            ServiceContainer.Register<RegistrationViewModel>(() => new RegistrationViewModel());

            var masterDetailViewModel = new MasterDetailViewModel();
            ServiceContainer.Register<MasterDetailViewModel>(() => masterDetailViewModel);

            MainPage = new MainPage();
            var master = new MasterDetail();
            MainPage = master;
            master.BindingContext = masterDetailViewModel;
            MainPage = ViewManager.ChangeView(typeof(PairViewModel));
         
        }
        private Task InitNavigation()
        {
            var navigationService = ServiceContainer.Resolve<INavigationService>();
            return navigationService.InitializeAsync();
        }
        protected async override void OnStart()
        {
            // Handle when your app starts
            base.OnStart();
            await InitNavigation();
            base.OnResume();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
