using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MdxClientPooled
{
    public class AdomdConnectionPool
    {
        private static readonly Dictionary<string, Dictionary<string, ArrayList>> ConnectionPool =
        new Dictionary<string, Dictionary<string, ArrayList>>();


        public static MdxPooledConnection GetConnection(string connectionString)
        {
            // Pooling (See the Poll method)
            ValidateListExistance(connectionString);
            return GetConnectionFromPool(connectionString);
        }
        private static MdxPooledConnection GetConnectionFromPool(string connectionString)
        {
            var session = new KeyValuePair<string, ArrayList>(null, null);
            lock (ConnectionPool[connectionString])
            {
                if (ConnectionPool[connectionString].Count > 0)
                {
                    // Available session exists. Use it.
                    session = ConnectionPool[connectionString].First();
                    ConnectionPool[connectionString].Remove(session.Key);
                }
            }
            // No available connections exist. Create a new one.
            if (session.Key == null)
                return CreateNewConnection(connectionString);
            // A session exists
            var pooledConnection = new MdxConnection(connectionString);
            pooledConnection.Connection.SessionID = session.Key;
            pooledConnection.Open();
            // Register the session with the pool.
            var poolItem = new MdxPooledConnection(pooledConnection, session.Value);
            poolItem.Disposed += PoolItemDisposed;
            return poolItem;
        }
        private static void ValidateListExistance(string connectionString)
        {
            lock (ConnectionPool)
            {
                if (!ConnectionPool.ContainsKey(connectionString))
                    ConnectionPool.Add(connectionString, new Dictionary<string, ArrayList>());
            }
        }
        private static MdxPooledConnection CreateNewConnection(string connectionString)
        {
            // Create a new connection and register it with the pool.
            var poolItem = new MdxPooledConnection(
                        new MdxConnection(connectionString), 
                        new ArrayList());
            poolItem.Disposed += PoolItemDisposed;
            poolItem.Connection.Open();
            return poolItem;
        }
        private static void PoolItemDisposed(MdxPooledConnection sender, EventArgs e)
        {
            Dictionary<string, ArrayList> connections = ConnectionPool[sender.Connection.ConnectionString];
            try
            {
                // Close the connection, but keep the session alive.
                sender.Connection.Connection.Close(false);
                lock (connections)
                {
                    if (sender.Connection.Connection.State == ConnectionState.Closed && !sender.IsBroken)
                    {
                        // Reclaim the connection to the pool.
                        connections.Add(sender.Connection.Connection.SessionID, sender.ExtraData);
                    }
                }
            }
            catch
            {
                // Can't close connection? Don't let it back in the pool.
                // We don't really care why, though. If necessary in the future, log.
            }
        }


    }
}
