/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: ConnectionManager.cs
			// Describle:  manage some different server-connection
			// Created By:  hsu
			// Date&Time:  2016/3/3 19:11:09
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Air2000
{
    public class ConnectionManager
    {
        #region [enum] ServerType
        public enum ServerType
        {
            Login, // login(account) server
            Logic, // game logic server
        }
        #endregion

        #region [Fields]
        private static ConnectionManager m_Instance; // single instance
        private Dictionary<ServerType, Connection> m_Connections; // all connected connection
        private List<Connection> m_Buffer; // temporary buffer
        #endregion

        #region [Functions]

        /// <summary>
        /// Get game connection manager's single instance.
        /// </summary>
        /// <returns></returns>
        public static ConnectionManager GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new ConnectionManager();
            }
            return m_Instance;
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        private ConnectionManager()
        {
            m_Connections = new Dictionary<ServerType, Connection>();
            m_Buffer = new List<Connection>();
        }

        /// <summary>
        /// Connect to a specific server with host and port number,we now have account server and game logic server.
        /// </summary>
        /// <param name="svrType"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool CreateConnection(ServerType svrType, string host, int port, Connection.StateListener listener)
        {
            DeleteConnection(svrType);
            Connection connection = new Connection();
            m_Connections.Add(svrType, connection);
            connection.SetNetStateListener(listener);
            return connection.Connect(host, port);
        }

        /// <summary>
        /// Disconnect form a server with server type,we now have account server and game logic server.
        /// </summary>
        /// <param name="svrType"></param>
        public void DeleteConnection(ServerType svrType)
        {
            Connection connection;
            if (m_Connections.TryGetValue(svrType, out connection))
            {
                connection.Disconnect();
                m_Connections.Remove(svrType);
            }
        }

        /// <summary>
        /// Get a connection with server type.
        /// </summary>
        /// <param name="svrType"></param>
        /// <returns>Connection</returns>
        public Connection GetConnection(ServerType svrType)
        {
            Connection connection;
            m_Connections.TryGetValue(svrType, out connection);
            return connection;
        }

        /// <summary>
        /// Drive manager,it's important to make sure all operations are operated in main thread.
        /// </summary>
        public void Update()
        {
            if (m_Buffer.Count != 0)
            {
                m_Buffer.Clear();
            }
            Dictionary<ServerType, Connection>.Enumerator it = m_Connections.GetEnumerator();
            for (int i = 0; i < m_Connections.Count; i++)
            {
                it.MoveNext();
                if (it.Current.Value != null)
                    it.Current.Value.Update();
            }
        }

        /// <summary>
        /// Dispose manager instance.
        /// </summary>
        public void Destroy()
        {
            Dictionary<ServerType, Connection>.Enumerator it = m_Connections.GetEnumerator();
            for (int i = 0; i < m_Connections.Count; i++)
            {
                it.MoveNext();
                Connection client = it.Current.Value;
                if (client != null)
                    client.Disconnect();
            }
            m_Connections.Clear();
            m_Connections = null;
        }

        #endregion
    }
}
