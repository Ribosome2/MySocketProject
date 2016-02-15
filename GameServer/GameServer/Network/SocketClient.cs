using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.NetWork
{
    public class SocketClient
    {
        public uint UniqueID;
        public Socket Socket;
       
        public bool Connected;
        public bool Lagging;
        public void Disconnect()
        {
            Connected = false;
            try
            {
                //todo cleanup
                Socket.Disconnect(false);
            }
            catch { }
        }
        public void Send(byte[] Data)
        {
            byte[] Dta = new byte[Data.Length];
            Buffer.BlockCopy(Data, 0, Dta, 0, Data.Length);

            try
            {
                this.Socket.Send(Dta);
            }
            catch { Disconnect(); }
        }
    }
}
