using System;
using System.Threading.Tasks;
using System.Windows.Input;
using BikeLockApp.Commands;
using BikeLockApp.Services;
using Xamarin.Forms.GoogleMaps;

namespace BikeLockApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        // Server related
        // TODO may have to move this to somewhere else 
        private const string address = "https://bikelockserver.azurewebsites.net/lockSwitch";
        ISocketConnection socket = new SocketConnection();


        // Properties 
        private string lockSwtichAction = "Lock Device"; // TODO Has to be changed so that it checks the state first
        public string LockSwitchAction
        {
            get { return lockSwtichAction; }
            set { lockSwtichAction = value; this.OnPropertyChanged(); }
        }

        private Position lockerPosition;
        public Position LockerPosition
        {
            get { return lockerPosition; }
            set { lockerPosition = value; this.OnPropertyChanged(); }
        }

        // GOOGLE MAPS

        public Map Map { get; set; } 







        private MapSpan region =
        MapSpan.FromCenterAndRadius(
        new Position(35.681298, 139.766247),
        Distance.FromKilometers(2)); // Initial position

        public MapSpan Region
        {
            get { return region; }
            set { region = value; this.OnPropertyChanged(); }
        }


        public LockState LockState { get; set; } = LockState.OFF; 


        


        // ICommand
        public ICommand LockSwitchCMD { get; set; }

        public HomeViewModel()
        {
            LockSwitchCMD = new CommandDelegate((Func<object, bool>)delegate
            {
                return true;

            }, (Action<object>)delegate {
                ChangeState(null);
            });

            Map = new Map
            {
                WidthRequest = 450,
                HeightRequest = 450,
                MapType = MapType.Hybrid
            };

            InitMap();





        }

        private async void InitMap()
        {
            // TODO lockerID should in the future be a variable saved somewhere
            LockerPosition = await socket.GetLockerPosition("DSFG32FDVA68FS", "https://bikelockserver.azurewebsites.net/position");

            if(LockerPosition.Latitude!=0 && LockerPosition.Longitude != 0)
            {
                Map.MoveToRegion(
                MapSpan.FromCenterAndRadius(lockerPosition, Distance.FromKilometers(0.03)));
                Pin lockerPin = new Pin
                {
                    Position = lockerPosition,
                    Label = "Your locker is here",
                    Type = PinType.Place
                };

                Map.Pins.Add(lockerPin);
            }


        }



        private async void ChangeState(object parameter)
        {
            SocketConnection socket = new SocketConnection(address, 1234);
            bool success; 

            // Will try to change the lock state, but only if the action is confirmed by the RPi 
            if (LockState == LockState.OFF)
            {
                success = await socket.ChangeState(LockState.ON);
                if (success)
                {
                    LockState = LockState.ON;
                    LockSwitchAction = "Unlock Device";
                }
                else { await App.Current.MainPage.DisplayAlert("Error", "Couldn't communicate with the device", "OK"); }
            } else if (LockState == LockState.ON)
            {
                success = await socket.ChangeState(LockState.OFF);
                if (success)
                {
                    LockState = LockState.OFF;
                    LockSwitchAction = "Lock Device";
                }
                else { await App.Current.MainPage.DisplayAlert("Error", "Couldn't communicate with the device", "OK"); }
            } 




        }


    }
}
