using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerUsingProtobuf;

namespace GameServer.NetWork
{


    public class GameServer:Singleton<GameServer>
    {
        private SocketAsyncEventArgsPool readWritePool;
        private Socket listenSocket;
        
        public GameServer()
        {
            Console.WriteLine("Initing GameServer");
            readWritePool = new SocketAsyncEventArgsPool(Constants.MaxConnections);
            SocketAsyncEventArgs readWriteEventArg;
            //
            #region 一开始准备好所有的异步参数池，
            for (int i = 0; i < Constants.MaxConnections; i++)
            {
                readWriteEventArg = new SocketAsyncEventArgs();
                readWriteEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                readWriteEventArg.SetBuffer(new byte[Constants.BufferSize], 0, Constants.BufferSize);
                readWriteEventArg.UserToken = new SocketClient();
                readWritePool.Push(readWriteEventArg);
            } 
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch(e.LastOperation)
            {
                case SocketAsyncOperation.Receive: ProcessReceive(e); break;
                default: throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }
        public void Start(string IP, ushort Port)
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(IP), Port);
            listenSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.ReceiveBufferSize = Constants.BufferSize;
            listenSocket.SendBufferSize = Constants.BufferSize;
            listenSocket.SendTimeout = 2000;
            listenSocket.ReceiveTimeout = 2000;
            listenSocket.Bind(ipe);
            listenSocket.Listen(Constants.MaxConnections);
            StartAccept(null);
        }

        /// <summary>
        /// 开始接收Client连接
        /// </summary>
        /// <param name="acceptEventArgs"></param>
        private void StartAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if (acceptEventArgs == null)
            {
                acceptEventArgs = new SocketAsyncEventArgs();
                acceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ProcessAccept);
            }
            else
            {
                acceptEventArgs.AcceptSocket = null;
            }


            bool handle = listenSocket.AcceptAsync(acceptEventArgs);
            if (!handle)
                ProcessAccept(null, acceptEventArgs);
        }

        /// <summary>
        /// 处理连接回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessAccept(object sender, SocketAsyncEventArgs e)
        {
            SocketAsyncEventArgs readEventArgs = readWritePool.Pop();
            //？？？？ 上面没看到赋值， 这个UseToken 的值是怎么来的， 怎么知道可以转成自定义的SocketClient？
            //后记：这个是在初始话对象池的时候new出来， 就赋值了的,
            SocketClient client = ((SocketClient)readEventArgs.UserToken);
            client.Socket = e.AcceptSocket;  //给Client赋予它管理的Socket
            try
            {
                //As soon as the client is connected, post a receive to the connection
                Console.WriteLine("got new client");
                bool handle = e.AcceptSocket.ReceiveAsync(readEventArgs);
                if (!handle)
                    ProcessReceive(readEventArgs);
                //继续侦听新的连接
                StartAccept(e);
            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString());
            }
        }

        /// <summary>
        /// 处理接收数据
        /// </summary>
        /// <param name="e"></param>
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            SocketClient client = (SocketClient)e.UserToken;
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                byte[] buffer = new byte[e.BytesTransferred];
                Console.WriteLine(string.Format("bytes {0} offset{1}", e.BytesTransferred, e.Offset));
                Buffer.BlockCopy(e.Buffer, e.Offset, buffer, 0, e.BytesTransferred);
                //上面只是复制了收到的字节，那在哪里重置缓存字节的偏移？ 
                //下面的ReceiveAsync()会从offset 0开始接收新的字节？ 
                //Todo : 写一个测试：输出当前的buffer offset值，调用一次ReceiveAsync之后再输出一次offset的值
                //就可以知道是不是底层帮封装好了 经验证：offfset的重置是封装好的

                PacketHandler.HandleBytes(buffer, client);
                bool handle = client.Socket.ReceiveAsync(e);
                if (!handle)
                    ProcessReceive(e);
            }
            else
                ProcessDisconnect(e);
        }
        private void ProcessDisconnect(SocketAsyncEventArgs e)
        {
            SocketClient client = e.UserToken as SocketClient;
            try
            {
                client.Socket.Shutdown(SocketShutdown.Send);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.Message);
            }
            client.Socket.Close();
            readWritePool.Push(e);
        }

    }
}
