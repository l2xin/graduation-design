using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace Air2000
{
    public delegate void HandleNetworkDelegate();
    public class GameClient
    {
        public abstract class NetStateListener
        {
            public HandleNetworkDelegate OnDisconnect;
            public HandleNetworkDelegate OnConnect;
            public HandleNetworkDelegate OnUpdate;

            public NetStateListener()
            {
                OnDisconnect = new HandleNetworkDelegate(Func);
                OnConnect = new HandleNetworkDelegate(Func);
                OnUpdate = new HandleNetworkDelegate(Func);
            }
            private void Func() { }
        }

        private Connect m_Connect;
        private bool m_IsConnected;
        private NetStateListener m_NetStateListener;

        public NetStateListener GetNetStateListener
        {
            get { return m_NetStateListener; }
        }

        private List<NetPacket> m_PacketsBuffer = new List<NetPacket>();

        public void SetNetStateListener(NetStateListener listener)
        {
            m_NetStateListener = listener;
        }

        public GameClient()
        {
            m_IsConnected = false;
        }

        public bool Connect(string host, int port)
        {
            Disconnect();
            m_Connect = new Air2000.Connect();
            m_Connect.OnConnectCallback = OnConnected;
            m_Connect.OnDisconnectCallback = OnDisconnected;
            bool b = m_Connect.DoConnect(host, port);
            return b;
        }

        public bool Reconnect()
        {
            if (m_Connect != null)
            {
                bool b = m_Connect.DoReconnect();
                return b;
            }
            return false;
        }

        private void OnConnected()
        {
            if (m_NetStateListener != null)
            {
                m_NetStateListener.OnDisconnect();
            }
        }
        private void OnDisconnected()
        {
            if (m_NetStateListener != null)
            {
                m_NetStateListener.OnConnect();
            }
        }
        public bool IsConnected()
        {
            if (m_Connect == null)
                return false;

            return m_Connect.IsConnected;
        }
        public void Update()
        {
            if (m_Connect != null)
            {
                if (m_Connect.NeedCallConnected && m_Connect.OnConnectCallback != null)
                {
                    m_Connect.NeedCallConnected = false;
                    m_Connect.OnConnectCallback();
                }
                else if (m_Connect.NeedCallDisconnected && m_Connect.OnDisconnectCallback != null)
                {
                    m_Connect.NeedCallDisconnected = false;
                    m_Connect.OnDisconnectCallback();
                    return;
                }

                if (m_Connect.m_NeedClose)
                {
                    m_Connect.DoClose();
                    return;
                }

                if (m_Connect.IsConnected)
                {
                    m_IsConnected = m_Connect.IsConnected;

                    if (m_PacketsBuffer.Count != 0)
                    {
                        m_PacketsBuffer.Clear();
                    }

                    m_Connect.GetAllReveivePackets(out m_PacketsBuffer);
                    if (m_PacketsBuffer.Count != 0)
                    {
                        for (int i = 0; i < m_PacketsBuffer.Count; i++)
                        {
                            NetPacket packet = m_PacketsBuffer[i];

                            if (packet.GetMessageID() > 9999)
                            {
                                //NetWorkEventManager.GetSingleton().NotifyNetWorkEvent(new NetWorkEventEx<NetPacket>((GameMessage)tempack.getMessageID(), packet));
                                continue;
                            }
                            //NetWorkEventManager.GetSingleton().NotifyNetWorkEvent(new NetWorkEventEx<NetPacket>((GameMessage)tempack.getMessageID(), packet));
                        }
                        m_PacketsBuffer.Clear();
                    }
                }
            }

            if (m_NetStateListener != null)
            {
                m_NetStateListener.OnUpdate();
            }
        }

        public bool SendMessage(int cmd, Byte[] data, int playerID)
        {
            if (m_Connect == null || m_Connect.IsConnected == false)
            {
                return false;
            }
            return m_Connect.SendMessage(cmd, playerID, data);
        }

        public bool SendMessageEmpty(int cmd)
        {
            return SendMessage(cmd, new byte[] { 0 }, 0);
        }

        public bool SendProtoBuf<T>(int cmd, T obj) where T : class, ProtoBuf.IExtensible
        {
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<T>(ms, obj);
                byte[] bytes = ms.ToArray();
                ms.Close();
                return SendMessage(cmd, bytes, 0);
            }
        }

        public static T DeserializeProtoBuf<T>(object obj) where T : class, ProtoBuf.IExtensible
        {
            NetPacket packet = obj as NetPacket;
            if (packet == null)
            {
                return null;
            }
            T t = DeserializeProtoBuf<T>(packet);
            return t;
        }
        public static T DeserializeProtoBuf<T>(NetPacket packet) where T : class, ProtoBuf.IExtensible
        {
            Byte[] buffer = packet.GetBuffer();
            if (buffer == null)
            {
                return null;
            }
            using (var ms = new MemoryStream(buffer, NetPacket.PACK_HEAD_SIZE, buffer.Length - NetPacket.PACK_HEAD_SIZE))
            {
                return ProtoBuf.Serializer.Deserialize(typeof(T), ms) as T;
            }
        }
    }
}
