/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: Connection.cs
			// Describle:  connection unit
			// Created By:  hsu
			// Date&Time:  2016/3/3 19:11:09
            // Modify History:
            //
//----------------------------------------------------------------*/

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace Air2000
{
    public class Connection
    {
        #region [Declration]
        public delegate void ConnectionDelegate(Connection connection);
        public class StateListener
        {
            public ConnectionDelegate OnConnect;
            public ConnectionDelegate OnUpdate;
            public ConnectionDelegate OnDisconnect;

            public StateListener()
            {
                OnDisconnect = new ConnectionDelegate(Func);
                OnConnect = new ConnectionDelegate(Func);
                OnUpdate = new ConnectionDelegate(Func);
            }
            private void Func(Connection connection) { }
        }

        private SocketConnect m_Socket; // socket connection
        private StateListener m_ConnectionStateListener; // net state listener
        private List<NetPacket> m_PacketsBuffer = new List<NetPacket>(); // packets' buffer
        #endregion

        #region [Functions]
        /// <summary>
        /// Get connection state listener.
        /// </summary>
        /// <returns></returns>
        public StateListener GetStateListener() { return m_ConnectionStateListener; }

        /// <summary>
        /// Set connection state listener.
        /// </summary>
        /// <param name="listener"></param>
        public void SetNetStateListener(StateListener listener) { m_ConnectionStateListener = listener; }

        public bool IsConnected()
        {
            if (m_Socket == null)
                return false;

            return m_Socket.IsConnected;
        }

        /// <summary>
        /// Execute connect.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Connect(string host, int port)
        {
            Disconnect();
            m_Socket = new Air2000.SocketConnect();
            m_Socket.OnConnectCallback = OnConnected;
            m_Socket.OnDisconnectCallback = OnDisconnected;
            return m_Socket.Connect(host, port);
        }

        /// <summary>
        /// Execute reconnect.
        /// </summary>
        /// <returns></returns>
        public bool Reconnect()
        {
            if (m_Socket != null)
            {
                bool b = m_Socket.Reconnect();
                return b;
            }
            return false;
        }

        /// <summary>
        /// Disconnect the socket.
        /// </summary>
        public void Disconnect()
        {
            if (m_Socket != null)
            {
                m_Socket.Close();
                m_Socket = null;
            }
        }

        private void OnConnected()
        {
            if (m_ConnectionStateListener != null)
            {
                m_ConnectionStateListener.OnDisconnect(this);
            }
        }

        private void OnDisconnected()
        {
            if (m_ConnectionStateListener != null)
            {
                m_ConnectionStateListener.OnConnect(this);
            }
        }

        public void Update()
        {
            if (m_Socket != null)
            {
                if (m_Socket.NeedCallConnected && m_Socket.OnConnectCallback != null)
                {
                    m_Socket.NeedCallConnected = false;
                    m_Socket.OnConnectCallback();
                }
                else if (m_Socket.NeedCallDisconnected && m_Socket.OnDisconnectCallback != null)
                {
                    m_Socket.NeedCallDisconnected = false;
                    m_Socket.OnDisconnectCallback();
                    return;
                }

                if (m_Socket.m_NeedClose)
                {
                    m_Socket.Close();
                    return;
                }

                if (m_Socket.IsConnected)
                {
                    if (m_PacketsBuffer.Count != 0)
                    {
                        m_PacketsBuffer.Clear();
                    }

                    m_Socket.GetAllReveivePackets(out m_PacketsBuffer);
                    if (m_PacketsBuffer != null && m_PacketsBuffer.Count != 0)
                    {
                        for (int i = 0; i < m_PacketsBuffer.Count; i++)
                        {
                            NetPacket packet = m_PacketsBuffer[i];
                            if (packet != null)
                                NetworkEventProcessor.GetInstance().Notify(new EventEX<NetPacket>(packet.GetMessageID(), packet));
                        }
                        m_PacketsBuffer.Clear();
                    }
                }
            }

            if (m_ConnectionStateListener != null)
            {
                m_ConnectionStateListener.OnUpdate(this);
            }
        }

        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="data"></param>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public bool SendMessage(int cmd, Byte[] data, int playerID)
        {
            if (m_Socket == null || m_Socket.IsConnected == false)
            {
                return false;
            }
            return m_Socket.SendMessage(cmd, playerID, data);
        }

        /// <summary>
        /// Send a empty message.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool SendMessageEmpty(int cmd)
        {
            return SendMessage(cmd, new byte[] { 0 }, 0);
        }

        /// <summary>
        /// Send a message and the data format is ProtoBuf.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool SendMessageProtoBuf<T>(int cmd, T obj) where T : class, ProtoBuf.IExtensible
        {
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<T>(ms, obj);
                byte[] bytes = ms.ToArray();
                ms.Close();
                return SendMessage(cmd, bytes, 0);
            }
        }

        /// <summary>
        /// Deserialize data(ProtoBuf.IExtensible).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Deserialize data(Proto.IExtensible).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="packet"></param>
        /// <returns></returns>
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

        #endregion

    }
}
