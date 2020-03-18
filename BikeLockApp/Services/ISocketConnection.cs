using System;
using System.Threading.Tasks;
using Xamarin.Forms.GoogleMaps;

namespace BikeLockApp.Services
{
    public enum LockState { ON, OFF };

    public interface ISocketConnection
    {
        void EstablishConnection(string address, int port);

        Task<bool> ChangeState(LockState lockState);

        void CheckState();

        Task<Position> GetLockerPosition(string lockerID, string address);

        Task<bool> CheckIfLockerUsed(string lockID, string address);
    }
}
