using System.Text;
using Konscious.Security.Cryptography;
using PHS.Networking.Enums;
using Tcp.NET.Client.Events.Args;

namespace DoorBirdController
{
    public class DoorBirdController
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly string _ipAddress;
        private bool _isLocked;

        public event EventHandler DoorBellTriggered;

        public DoorBirdController(string ipAddress)
        {
            _ipAddress = ipAddress;
        }

        /// <summary>
        /// Starts monitoring the doorbell status continuously.
        /// </summary>
        public async Task StartDoorBellMonitoring()
        {
            while (true)
            {
                await Task.Delay(2000);
                await CheckDoorBellStatus();
            }
        }

        /// <summary>
        /// Checks the current status of the doorbell.
        /// </summary>
        public async Task CheckDoorBellStatus()
        {
            UDPData udpData = new UDPData();
            string url = $"http://{_ipAddress}/bha-api/monitor.cgi?ring=doorbell";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string doorBellEvent = udpData.Event;
                OnDoorBellTriggered(doorBellEvent);
            }
        }
        
        protected virtual void OnDoorBellTriggered(string doorBellEvent)
        {
            DoorBellTriggered?.Invoke(this, new DoorBellEventArgs(doorBellEvent));
        }

        public class DoorBellEventArgs : EventArgs
        {
            public string DoorBellEvent { get; }

            public DoorBellEventArgs(string doorBellEvent)
            {
                DoorBellEvent = doorBellEvent;
            }
        }

        /// <summary>
        /// Optional event that can be subscribed to if connection tracking is needed outside of this program
        /// </summary>
        private async void OnConnectionEvent(object sender, TcpConnectionClientEventArgs e)
        {
            switch (e.ConnectionEventType)
            {
                case ConnectionEventType.Connected:
                    break;
                case ConnectionEventType.Disconnect:
                    break;
            }
        }

        /// <summary>
        /// Sets the lock state of the door.
        /// </summary>
        /// <param name="locked">True to lock the door, false to unlock.</param>
        public async Task SetLockState(bool locked)
        {
            int value = locked ? 1 : 2;
            string url = $"http://{_ipAddress}/bha-api/open-door.cgi?r={value}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                _isLocked = locked;
                Console.WriteLine($"Door {(locked ? "locked" : "unlocked")} successfully.");
            }
            else
            {
                Console.WriteLine("Failed to set the lock state.");
            }
        }

        public bool IsLocked
        {
            get => _isLocked;
            set => _isLocked = value;
        }

        /// <summary>
        /// Requests the live video feed from the doorbell.
        /// </summary>
        public async Task RequestVideoFeed()
        {
            string url = $"http://{_ipAddress}/bha-api/video.cgi";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Live video request successful.");
            }
            else
            {
                Console.WriteLine("Failed to request live video feed.");
            }
        }

        /// <summary>
        /// Requests a live image from the doorbell.
        /// </summary>
        public async Task RequestLiveImage()
        {
            string url = $"http://{_ipAddress}/bha-api/image.cgi";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Live image request successful.");
            }
            else
            {
                Console.WriteLine("Failed to request live image.");
            }
        }

        /// <summary>
        /// Energizes the light relay of the device.
        /// </summary>
        public async Task TurnOnLight()
        {
            string url = $"http://{_ipAddress}/bha-api/light-on.cgi";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Light turned on successfully.");
            }
            else
            {
                Console.WriteLine("Failed to turn on the light.");
            }
        }

        /// <summary>
        /// Requests a history image from the doorbell.
        /// </summary>
        /// <param name="index">Index of the history image (1-50).</param>
        /// <param name="eventType">Event type (doorbell or motionsensor).</param>
        public async Task RequestHistoryImage(int index, string eventType = "doorbell")
        {
            string url = $"http://{_ipAddress}/bha-api/history.cgi?index={index}&event={eventType}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("History image request successful.");
            }
            else
            {
                Console.WriteLine("Failed to request history image.");
            }
        }

        /// <summary>
        /// Extracts the salt value from the UDP packet data.
        /// </summary>
        /// <param name="packetData">The UDP packet data.</param>
        /// <returns>The extracted salt value.</returns>
        private byte[] ExtractSalt(byte[] packetData)
        {
            UDPPacket saltPacket = new UDPPacket();
            saltPacket.ParsePacket(packetData);
            return saltPacket.SALT;
        }

        /// <summary>
        /// Extracts the OpsLimit value from the UDP packet data.
        /// </summary>
        /// <param name="packetData">The UDP packet data.</param>
        /// <returns>The extracted OpsLimit value.</returns>
        private byte[] ExtractOpsLimit(byte[] packetData)
        {
            UDPPacket opsPacket = new UDPPacket();
            opsPacket.ParsePacket(packetData);
            return opsPacket.OPSLIMIT;
        }

        /// <summary>
        /// Extracts the MemLimit value from the UDP packet data.
        /// </summary>
        /// <param name="packetData">The UDP packet data.</param>
        /// <returns>The extracted MemLimit value.</returns>
        private byte[] ExtractMemLimit(byte[] packetData)
        {
            UDPPacket memPacket = new UDPPacket();
            memPacket.ParsePacket(packetData);
            return memPacket.MEMLIMIT;
        }

        /// <summary>
        /// Generates a password hash using the Argon2id algorithm.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt value to use in the hashing process.</param>
        /// <returns>The generated password hash.</returns>
        private byte[] GeneratePasswordHash(string password, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 8;
            argon2.Iterations = 4;
            argon2.MemorySize = 1024 * 1024;

            return argon2.GetBytes(16);
        }

        /// <summary>
        /// Generates a stretched password using the Argon2i algorithm.
        /// </summary>
        /// <param name="password">The password to stretch.</param>
        /// <returns>The generated stretched password.</returns>
        public byte[] GenerateStretchedPassword(string password)
        {
            string passwordPrefix = password.Substring(0, 5);

            byte[] salt = new byte[]
            {
                0x77, 0x35, 0x36, 0xDC, 0xC3, 0x0E, 0x2E, 0x84,
                0x7E, 0x0E, 0x75, 0x29, 0xE2, 0x34, 0x60, 0xCF
            };

            var argon2 = new Argon2i(Encoding.UTF8.GetBytes("QzT3j"));

            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 1;
            argon2.Iterations = 4;
            argon2.MemorySize = 8192;

            return argon2.GetBytes(32);
        }
    }
}