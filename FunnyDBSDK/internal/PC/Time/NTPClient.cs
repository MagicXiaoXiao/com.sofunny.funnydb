using System;
using System.Net;
using System.Net.Sockets;
namespace SoFunny.FunnyDB.PC
{
    public class NTPClient
    {
        internal DateTime RequestTime(string host, int timeout)
        {
            IPAddress[] addresses = Dns.GetHostAddresses(host);
            IPEndPoint ipEndPoint = new IPEndPoint(addresses[0], 123);
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                var ntpData = new byte[48];
                ntpData[0] = 0x1B;

                socket.Connect(ipEndPoint);
                socket.ReceiveTimeout = timeout;
                socket.Send(ntpData);
                socket.Receive(ntpData);

                const byte serverReplyTime = 40;
                ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
                ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

                intPart = EnsureSwapEndianness(intPart);
                fractPart = EnsureSwapEndianness(fractPart);
                var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
                var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);
                return networkDateTime;
            }
        }

        private uint EnsureSwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) + ((x & 0x0000ff00) << 8) + ((x & 0x00ff0000) >> 8) + ((x & 0xff000000) >> 24));
        }

    }
}

