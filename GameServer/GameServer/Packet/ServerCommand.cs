using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Packet
{
    /// <summary>
    /// 服务器发给客户端的协议基类
    /// </summary>
    abstract class CommandBase
    {
        public int commandCode;
    }
}
