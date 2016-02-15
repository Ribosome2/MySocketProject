using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerUsingProtobuf;
using GameServer.Packet;
namespace GameServer.NetWork
{
    class PacketHandler
    {
        public static void HandleBytes(byte[] data, SocketClient srcClient)
        {
            Console.WriteLine("echo back to client " + srcClient+data.Length);
            srcClient.Send(data);
            
        }
 
        //public void SendToClient(ServerConmand)
    }
}
