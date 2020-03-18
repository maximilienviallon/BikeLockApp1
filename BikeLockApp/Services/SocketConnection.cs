using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BikeLockApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms.GoogleMaps;

namespace BikeLockApp.Services
{
    public class SocketConnection : ISocketConnection
    {

        // Personal information relevant for HTTP requests

        private string email = "lukas.duus@hotmail.com";
        private string attachedLockID = "123"; // This will be a unique ID for the locker itself

        // Server info
        private string address;
        private int port;

        // Client
        private HttpClient client;

        public SocketConnection(string address, int port)
        {
            this.address = address;
            this.port = port;

            client = new HttpClient();
            // The amount of time it should wait
            // client.Timeout = TimeSpan.FromSeconds(5);

        }

        public SocketConnection()
        {
            client = new HttpClient();
        }


        public void EstablishConnection(string address, int port)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ChangeState(LockState lockState)
        {
            string setting;
            if (lockState == LockState.ON)
            {
                setting = "ON";
            }
            else { setting = "OFF"; }

            try
            {
                var post = new LockSwitch { Setting = setting, Email = email, AttachedLockID = attachedLockID };
                var content = JsonConvert.SerializeObject(post);
                Console.WriteLine(content.ToString());
                var response = await client.PostAsync(address, new StringContent(content, Encoding.UTF8, "application/json"));


                string body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);


                JObject jObj = JObject.Parse(body);
                Console.WriteLine("Was parsing the body to a JObject successful: " + jObj != null);
                if (jObj != null)
                {
                    // This part will compare the requested lockState with the lockState responded by the IOT device
                    // We do this to ensure that the locking/unlocking was actually done
                    string state = (string)jObj.SelectToken("state");
                    if (lockState == LockState.ON && state.Equals("locked") || lockState == LockState.OFF && state.Equals("unlocked"))
                    {
                        Console.WriteLine("Changed state returned true");
                        return true;
                    }

                }
            }
            catch (Newtonsoft.Json.JsonReaderException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return false;
        }

        public async Task<bool> CheckIfLockerUsed(string lockerID, string address)
        {
            var post = new { attachedLockID = lockerID };
            var content = JsonConvert.SerializeObject(post);
            Console.WriteLine(content);
            try
            {
                var response = await client.PostAsync(address, new StringContent(content, Encoding.UTF8, "application/json"));

                string body = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response: " + body);
                JObject jObj = JObject.Parse(body);
                if (jObj != null)
                {
                    string used = (string)jObj.SelectToken("used");
                    if (used.Equals("no"))
                    {
                        return false;
                    }

                }
            }
            catch (System.Threading.Tasks.TaskCanceledException ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Will reject you if jObj for some reason is null or if used is equal to "yes"
            return true;

        }

        public void CheckState()
        {

            throw new NotImplementedException();
        }

        public async Task<Position> GetLockerPosition(string lockerID, string address)
        {
            Position lockerPosition = new Position();
            var post = new { LockerID = lockerID };
            var content = JsonConvert.SerializeObject(post);
            Console.WriteLine(content);
            try
            {
                Console.WriteLine("The address: " + address);
                var response = await client.PostAsync(address, new StringContent(content, Encoding.UTF8, "application/json"));
                
                string body = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response: " + body);
                JObject jObj = JObject.Parse(body);
                if (jObj != null)
                {
                    // Will try parsing the result to a double, since the returnVal of the json is a string
                    double.TryParse((string)jObj.SelectToken("lat"), out double latitude);
                    double.TryParse((string)jObj.SelectToken("lon"), out double longtitude);

                    lockerPosition = new Position(latitude, longtitude);

                }
            }
            catch (Exception ex)
            {
                // Will be handled differently in the future
                Console.WriteLine(ex.Message);
                await App.Current.MainPage.DisplayAlert("ERROR", ex.Message, "OK");
            }


            Console.WriteLine("Returned: Position(lat: {0}, long: {1})",lockerPosition.Latitude, lockerPosition.Longitude);
            return lockerPosition;
        }
    }
}
