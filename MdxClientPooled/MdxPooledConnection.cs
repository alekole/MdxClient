using System;
using System.Collections;

namespace MdxClient
{
    /// <summary>
    /// Реализация соединения с Olap с использованием пула
    /// </summary>
    public class MdxPooledConnection : IDisposable
    {
        internal MdxPooledConnection(MdxConnection connection, ArrayList extraData)
        {
            Connection = connection;
            ExtraData = extraData;
        }

        public MdxConnection Connection { get; private set; }
        public ArrayList ExtraData { get; private set; }
        void IDisposable.Dispose()
        {
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }
        internal event Action<MdxPooledConnection, EventArgs> Disposed;
    }
}
