using System;
using System.Windows.Input;
using BikeLockApp.Commands;
using BikeLockApp.Services;
using BikeLockApp.Views;
using BikeLockApp.Views.QRScanner;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace BikeLockApp.ViewModels
{
    public class PairViewModel : BaseViewModel
    {

        // Server related
        // TODO may have to move this to somewhere else 
        private const string address = "https://bikelockserver.azurewebsites.net/users";

        private ISocketConnection socket = new SocketConnection(address, 1234);
        


        public ICommand OpenQRCMD { get; set; }

        public ICommand ConfirmCMD { get; set; }

        private string lockID;
        public string LockID
        {
            get { return lockID; }
            set { lockID = value; this.OnPropertyChanged(); }
        }

        public PairViewModel()
        {
            OpenQRCMD = new CommandDelegate((Func<object, bool>)delegate
            {
                return true;

            }, (Action<object>)delegate {

                var scan = new PartialScreenScanning();

                App.Current.MainPage = scan;
                scan.ZXigQRScanner.OnScanResult += (result) =>
                  {
                      LockID = result.Text;
                      Console.WriteLine("Done scanning. ID is: " + result.Text);
                      Device.BeginInvokeOnMainThread(async () =>
                      {
                          App.Current.MainPage = ViewManager.ChangeView(typeof(PairViewModel));
                          scan.ZXigQRScanner.IsScanning = false;
                      });
                  };
                

            });

            ConfirmCMD = new CommandDelegate((Func<object, bool>)delegate
            {
                return true;

            }, (Action<object>)delegate {
                ConfirmDetails();
               //  App.Current.MainPage = ViewManager.ChangeView(typeof(HomeViewModel));
            });


        }

        /// <summary>
        /// Validation and submitting the info 
        /// </summary>
        private async void ConfirmDetails()
        {

            bool isUsed = await socket.CheckIfLockerUsed(LockID,address);

            Console.WriteLine("Is this locker used: " + isUsed);

            if (!isUsed)
            {
                // TEMPORARY! JUST TO GET INTO THE OTHER VIEWS
                App.Current.MainPage = ViewManager.ChangeView(typeof(HomeViewModel));



                // TODO Create a socket method, that adds the LockerID to the current user
                // The current user must be the one who is signed in and this may have to
                // be done with SQLlite or a static Data class (not smart)

                // BEFORE WE CONTINUE, IT IS IMPORTANT THAT REGISTRATION AND USER ACCOUNTS WORK



            } else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Sorry but this locker is already in use or doesn't exist. Please check the number and try again", "OK");
            }
          

            // Check if the LockID is valid (if its used by anyone else)

            // If yes, give an error TODO 

            // If no, update the users record in mongo DB with the LockID
            // so that you can see this person has access to that specific lock

            // ALTERNATIVE FLOW (Requires bluetooth):

            // If no one is using this LockID send the bluetooth mac-address to the RPi
            // And let the raspberry connect to your phone
            // If connection is established make the raspberry send a confirmation to the web api
            // And the web api send a confirmation to the client (this method)
            // Save the bluetooth mac-address in the 'users' record with the LockID


        }


    }







}
