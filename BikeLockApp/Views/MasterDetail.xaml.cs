using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using BikeLockApp.Models;
using BikeLockApp.ViewModels;
using System.Reflection;

namespace BikeLockApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterDetail : MasterDetailPage
    {
        public MasterDetail()
        {
            InitializeComponent();
            profileImage.Source = ImageSource.FromFile("spider.jpg");

            navigationList.ItemsSource = GetMenuList();

            IsPresented = false;
        }

        public List<MasterMenuItems> GetMenuList()
        {
            List<MasterMenuItems> list = new List<MasterMenuItems>();

            list.Add(new MasterMenuItems()
            {
                Text = "Login",
                Detail = "",
                ImagePath = "",
                TargetViewModel = typeof(LoginViewModel)
            });

            list.Add(new MasterMenuItems()
            {
                Text = "Pairing",
                Detail = "",
                ImagePath = "",
                TargetViewModel = typeof(PairViewModel)
            });

            return list;
        }

        private void OnMenuItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var selectedMenuItem = (MasterMenuItems)e.SelectedItem;

            var viewModel = (MasterDetailViewModel)this.BindingContext;
            viewModel.ChangeVMCMD.Execute(selectedMenuItem);

            IsPresented = false;
        }
    }
}