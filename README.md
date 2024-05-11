A C# library that provides a convenient way to interact with DoorBird video door stations and BirdGuard devices using their LAN-2-LAN API. This library simplifies the integration of DoorBird devices into your own applications, allowing you to control and monitor various aspects of the device programmatically.
Key Features:

Doorbell Monitoring: Continuously monitor the doorbell status and receive real-time notifications when the doorbell is triggered.
Lock Control: Remotely lock or unlock the door associated with the DoorBird device.
Video and Image Streaming: Request live video feeds and capture live images from the DoorBird device.
Light Control: Turn on the light relay of the DoorBird device remotely.
History Access: Retrieve history images from the DoorBird device based on specific event types.
Password Hashing: Generate secure password hashes using the Argon2id and Argon2i algorithms.
UDP Packet Parsing: Extract relevant information from UDP packet data, such as salt, OpsLimit, and MemLimit values.

The DoorBird Controller library leverages the following APIs and technologies:

DoorBird LAN-2-LAN API: The library communicates with DoorBird devices using the LAN-2-LAN API, which allows for local network communication and control of the device.
HTTP/HTTPS: The library utilizes HTTP/HTTPS protocols to send requests and receive responses from the DoorBird device.
Argon2: The library incorporates the Argon2 password hashing algorithm (Argon2id and Argon2i) for secure password storage and verification.

To use the DoorBird Controller library, simply instantiate the DoorBirdController class with the IP address of your DoorBird device. You can then utilize the various methods provided by the library to interact with the device, such as monitoring the doorbell status, controlling the lock, requesting video feeds, and more.
The DoorBird Controller library is designed to be easy to use and highly extensible. It provides a solid foundation for building applications that integrate with DoorBird devices, whether it's for home automation, security systems, or any other domain that requires remote control and monitoring capabilities.
Please note that this library is intended for use with DoorBird and BirdGuard devices only and requires a compatible firmware version installed on the device.
Contributions, bug reports, and feature requests are welcome! If you encounter any issues or have suggestions for improvement, please open an issue on the GitHub repository.
