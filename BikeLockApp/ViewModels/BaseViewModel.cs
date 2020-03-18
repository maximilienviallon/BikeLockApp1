using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using BikeLockApp.Services;

namespace BikeLockApp.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        protected readonly INavigationService NavigationService;

        internal static string UserName = "";
        public event PropertyChangedEventHandler PropertyChanged;

        public BaseViewModel()
        { 
            NavigationService = ViewModelLocator.Resolve<INavigationService>();
            var settingsService = ViewModelLocator.Resolve<ISettingsService>();
        }
        public void OnPropertyChanged([CallerMemberName]string name = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;
            changed(this, new PropertyChangedEventArgs(name));
        }
        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(false);
        }
    }
}
