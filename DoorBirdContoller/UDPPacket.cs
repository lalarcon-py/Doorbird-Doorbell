namespace DoorBirdController;

public class UDPPacket
{
    public byte[] IDENT { get; set; }
    public byte[] VERSION { get; set; }
    public byte[] OPSLIMIT { get; set; }
    public byte[] MEMLIMIT { get; set; }
    public byte[] SALT { get; set; }
    public byte[] NONCE { get; set; }
    public byte[] CIPHERTEXT { get; set; }
    public byte[] CapturedUDPPacket { get; set; }

    public void ParsePacket(byte[] PacketData)
    {
        if (PacketData.Length < 71)
        {
            throw new ArgumentException("Invalid packet data length.");
        }

        IDENT = new byte[] { PacketData[0], PacketData[1], PacketData[2] };
        VERSION = new byte[] { PacketData[3] };
        OPSLIMIT = new byte[] { PacketData[4], PacketData[5], PacketData[6], PacketData[7] };
        MEMLIMIT = new byte[] { PacketData[8], PacketData[9], PacketData[10], PacketData[11] };
        SALT = new byte[16];
        Array.Copy(PacketData, 12, SALT, 0, 16);
        NONCE = new byte[8];
        Array.Copy(PacketData, 28, NONCE, 0, 8);
        CIPHERTEXT = new byte[35];
        Array.Copy(PacketData, 36, CIPHERTEXT, 0, 35);

        CapturedUDPPacket = IDENT
            .Concat(VERSION)
            .Concat(OPSLIMIT)
            .Concat(MEMLIMIT)
            .Concat(SALT)
            .Concat(NONCE)
            .Concat(CIPHERTEXT)
            .ToArray();
    }
}