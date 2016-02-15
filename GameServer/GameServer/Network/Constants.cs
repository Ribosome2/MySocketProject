using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerUsingProtobuf
{
    public static class Constants
    {
        public const int MaxConnections = 300;
        public const int BufferSize = 1024;
        public const string IPAddress = "127.0.0.1";
        public const string CommandOperator = "/";
        public const ushort Port = 9090;
        public const byte ScreenDistance = 18;

        public const uint Min_MobUID = 400001;
        public const uint Max_MobUID = 499999;

        public const uint Min_PlayerUID = 1000000;
        public const uint Max_PlayerUID = 1999999999;

        public const uint MaxMoney = 999999999;
    }
}
