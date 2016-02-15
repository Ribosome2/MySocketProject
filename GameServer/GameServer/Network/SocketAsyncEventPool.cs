using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.NetWork
{
    /// <summary>
    /// Socket 异步时间参数池， 优化性能用
    /// </summary>
    public sealed class SocketAsyncEventArgsPool : IDisposable
    {
        private Stack<SocketAsyncEventArgs> m_pool;

        public SocketAsyncEventArgsPool(int capacity)
        {
            m_pool = new Stack<SocketAsyncEventArgs>(capacity);
        }
        public int Count
        {
            get { return m_pool.Count; }
        }
        public void Push(SocketAsyncEventArgs item)
        {
            lock (m_pool)
            {
                m_pool.Push(item);
            }
        }
        public SocketAsyncEventArgs Pop()
        {
            lock (m_pool)
            {
                return m_pool.Pop();
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            m_pool.Clear();
        }
        #endregion
    }
}
