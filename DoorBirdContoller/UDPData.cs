using System.Text;

namespace DoorBirdController;

public class UDPData
{
    
    // COMES IN AS BYTES - CONVERT TO STRING
    public string IntercomID { get; set; }
    
    // COMES IN AS BYTES - CONVERT TO STRING
    public string Event { get; set; }
    
    // COMES IN AS A NUMBER - CONVERT TO DATETIME
    public DateTimeOffset TimestampData { get; set; }

    public void ParseData(byte[] Data)
    {
        if (Data.Length < 18)
        {
            throw new ArgumentException("Invalid data length.");
        }

        // Parse data into INTERCOMID, Event, and TimestampData
        IntercomID = Encoding.UTF8.GetString(Data, 0, 6);
        Event = Encoding.UTF8.GetString(Data, 6, 8);

        byte[] timestampBytes = new byte[4];
        Array.Copy(Data, 14, timestampBytes, 0, 4);
        Array.Reverse(timestampBytes); // Reverse bytes to match little-endian
        int timestampValue = BitConverter.ToInt32(timestampBytes, 0);
        TimestampData = DateTimeOffset.FromUnixTimeSeconds(timestampValue);
    }
}