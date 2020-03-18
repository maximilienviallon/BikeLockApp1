using System;
using System.Collections.Generic;
using BikeLockApp.Services;
using BikeLockApp.ViewModels;
using Xamarin.Forms;
using ZXing;
using ZXing.Net.Mobile.Forms;

namespace BikeLockApp.Views.QRScanner
{
    public partial class PartialScreenScanning : ContentPage
    {
		public ZXingScannerView ZXigQRScanner { get; set; }

        public PartialScreenScanning()
        {
            // Get a reference to the Scanner so that we can manually assign events from the ViewModel
			InitializeComponent();
            ZXigQRScanner = _scanView;
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            // Yes we are breaking the MVVM pattern
            App.Current.MainPage = ViewManager.ChangeView(typeof(PairViewModel));
        }
    }
}
