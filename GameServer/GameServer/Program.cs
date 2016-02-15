using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerUsingProtobuf;
namespace GameServer.NetWork
{
    class Program
    {
        static void Main(string[] args)
        {
            GameServer.instance.Start(Constants.IPAddress, Constants.Port);
            Console.WriteLine("Listening for connections.");
            while (true)
            {
                while (true) Console.ReadKey();
            }
        }
    }
}
