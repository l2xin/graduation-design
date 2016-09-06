using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Air2000
{
    public class ClientManager
    {
        public enum ServerType {
            Login,
            Logic,
        }

        private static ClientManager m_Instance;

        private Dictionary<ServerType, GameClient> m_Clients;

        private List<GameClient> m_Buffer = new List<GameClient>();

        public static ClientManager GetInstance()
        {
            if(m_Instance == null)
            {
                m_Instance = new ClientManager();
            }
            return m_Instance;
        }
        public ClientManager()
        {
            m_Clients = new Dictionary<ServerType, GameClient>();
        }
        public bool Connect(ServerType svrType,string host,int port,GameClient.NetStateListener listener)
        {
            Disconnect(svrType);
            GameClient client = new GameClient();
            m_Clients.Add(svrType, client);
        }
    }
}
