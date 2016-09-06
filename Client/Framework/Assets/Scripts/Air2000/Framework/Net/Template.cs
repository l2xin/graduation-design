 
using System.Collections.Generic;
using UnityEngine;

namespace NCSpeedLight
{
    public class ClientManager
    {
        public enum ClientId
        {
            Login,
            Logic,
            BattleAcross,
            RecordChat,
        }

        private static ClientManager mManager;
        /// <summary>
        /// 所有客户端
        /// </summary>
        private Dictionary<ClientId, GameClient> mClients;

        /// <summary>
        /// The m_ buffer.
        /// </summary>
        List<GameClient> m_Buffer = new List<GameClient>();

        public static ClientManager GetManager()
        {
            if (mManager == null)
            {
                mManager = new ClientManager();
            }

            return mManager;
        }

        public ClientManager()
        {
            mClients = new Dictionary<ClientId, GameClient>();
        }

        public bool Connect(ClientId id, string host, int port, GameClient.NetStateListner listener)
        {
#if UNITY_WEBPLAYER
			UnityEngine.Security.PrefetchSocketPolicy(host, 843);
#endif

            Disconnect(id);

            //
            Helper.Log("new GameClient");
            GameClient c = new GameClient();
            mClients.Add(id, c);
            c.SetNetStateListner(listener);
            return c.Connect(host, port);
        }

        /*
		public void Connect(ClientId id, string host, int port)
		{
            Connect(id, host, port, null);
		}
		*/

        public void Disconnect(ClientId id)
        {
            GameClient c;
            if (mClients.TryGetValue(id, out c))
            {
                c.Disconnect();
                mClients.Remove(id);
            }
        }
        public GameClient GetClient(ClientId id)
        {
            GameClient c;
            mClients.TryGetValue(id, out c);
            return c;
        }

        public void Update()
        {
            //List<GameClient> buffer = new List<GameClient>();
            if (m_Buffer.Count != 0)
            {
                m_Buffer.Clear();
            }


            Dictionary<ClientId, GameClient>.Enumerator it = mClients.GetEnumerator();

            for (int i = 0; i < mClients.Count; i++)
            {
                it.MoveNext();
                m_Buffer.Add(it.Current.Value);
            }





            if (m_Buffer.Count != 0)
            {
                for (int c = 0; c < m_Buffer.Count; ++c)
                {
                    GameClient cc = m_Buffer[c];
                    if (cc != null)
                    {

                        cc.Update();
                    }
                }
                m_Buffer.Clear();
            }
        }

        public void OnDestroy()
        {
            foreach (KeyValuePair<ClientId, GameClient> p in mClients)
            {
                p.Value.Disconnect();
            }
            mClients.Clear();
        }

        public void SendMessageEmpty(GameMessage msgId)
        {
            GameClient client = null;
            if (msgId > GameMessage.GM_ACCOUNT_SERVER_MESSAGE_START && msgId < GameMessage.GM_ACCOUNT_SERVER_MESSAGE_END)
            {
                mClients.TryGetValue(ClientId.Login, out client);
            }
            else if (msgId > GameMessage.GATE_MESSAGE_CROSS_SERVER_BEGIN && msgId < GameMessage.GATE_MESSAGE_CROSS_SERVER_END)
            {
                if (mClients.TryGetValue(ClientId.BattleAcross, out client) == false)
                {
                    mClients.TryGetValue(ClientId.Logic, out client);
                }
            }
            else
            {
                mClients.TryGetValue(ClientId.Logic, out client);
            }
            if (client != null)
            {
                client.SendMessageEmpty((int)msgId);
                //Helper.Log("sendPack" + msgId);
            }
        }
    }

}
