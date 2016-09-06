/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: Net.cs
			// Describle:  
			// Created By:  hsu
			// Date&Time:  2016/3/3 19:11:09
            // Modify History:
            //
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;

namespace Air2000
{
    public delegate void HandleConnectDelegate();
    public class Connect
    {
        private Socket m_Socket; // socket instance
        private string m_IP; // address
        private int m_Port; // port number
        private bool m_IsConnected; // is connect to server
        private Byte[] m_ReceivedHeader;// received header
        public bool m_NeedClose;// whether need to close

        private LinkedList<NetPacket> m_WillBeSentPackets; // going to be sent packets
        private System.Object m_SendObject; // the send locked object
        private ManualResetEvent m_SendEvent;

        private LinkedList<NetPacket> m_ReceivedPackets; // received packets
        private System.Object m_ReceiveObject; // the receive locked object

        private HandleConnectDelegate m_ConnectFunction;
        private HandleConnectDelegate m_DisconnectFunction;

        private bool m_NeedCallConnected;
        private bool m_NeedCallDisconnected;

        public HandleConnectDelegate OnConnectCallback
        {
            get { return m_ConnectFunction; }
            set { m_ConnectFunction = value; }
        }
        public HandleConnectDelegate OnDisconnectCallback
        {
            get { return m_DisconnectFunction; }
            set { m_DisconnectFunction = value; }
        }
        public bool NeedCallConnected
        {
            get { return m_NeedCallConnected; }
            set { m_NeedCallConnected = value; }
        }
        public bool NeedCallDisconnected
        {
            get { return m_NeedCallDisconnected; }
            set { m_NeedCallDisconnected = value; }
        }

        public bool IsConnected
        {
            get { return m_IsConnected; }
        }

        public Connect()
        {
            m_IsConnected = false;
            m_NeedClose = false;
        }
        ~Connect() { DoClose(); }

        public void DoClose()
        {
            m_NeedClose = false;
            if (m_Socket != null)
            {
                try
                {
                    m_NeedCallDisconnected = true;
                    m_Socket.Shutdown(SocketShutdown.Both);
                    m_Socket.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError("Close socket fail,exception detail: " + e.Message);
                }
            }
            m_Socket = null;
            m_IsConnected = false;
        }

        /// <summary>
        /// Connect to server with ip and port number
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="portNumber"></param>
        /// <returns></returns>
        public bool DoConnect(string ip, int portNumber)
        {
            if (m_IsConnected)
            {
                DoClose();
            }

            m_IP = ip;
            m_Port = portNumber;
            m_NeedCallConnected = false;
            m_NeedCallDisconnected = false;

            IPAddress[] addresses = null;
            try
            {
                addresses = Dns.GetHostAddresses(ip);
            }
            catch (Exception e)
            {
                Debug.LogError("DNS error,detail: " + e.Message);
                if (m_DisconnectFunction != null)
                {
                    m_DisconnectFunction();
                }
                return false;
            }

            if (addresses == null || addresses.Length == 0)
            {
                if (m_DisconnectFunction != null)
                {
                    m_DisconnectFunction();
                }
                return false;
            }
            try
            {
                IPEndPoint formatIP = new IPEndPoint(addresses[0], portNumber);
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_Socket.BeginConnect(formatIP, new AsyncCallback(OnConnectedServer), m_Socket);
                m_IsConnected = false;
            }
            catch (Exception e)
            {
                if (m_DisconnectFunction != null)
                {
                    m_DisconnectFunction();
                }
                return false;
            }
            return true;
        }
        public bool DoReconnect()
        {
            return DoConnect(m_IP, m_Port);
        }
        private void OnConnectedServer(IAsyncResult result)
        {
            Socket skt = (Socket)result.AsyncState;
            if (skt.Connected)
            {
                m_IsConnected = true;
                m_NeedCallConnected = true;
                m_NeedCallDisconnected = false;

                InitNetworkThread();
            }
            else
            {
                m_IsConnected = false;
                m_NeedCallDisconnected = true;
                m_NeedCallConnected = false;
            }
        }
        private bool InitNetworkThread()
        {
            if (m_WillBeSentPackets == null)
            {
                m_WillBeSentPackets = new LinkedList<NetPacket>();
            }
            if (m_SendObject == null)
            {
                m_SendObject = new System.Object();
            }
            m_SendEvent = new ManualResetEvent(false);

            m_ReceivedPackets = new LinkedList<NetPacket>();
            m_ReceiveObject = new System.Object();

            Thread receiveThread = new Thread(ReceiveThreadLooperFunc);
            receiveThread.Start();

            Thread sendThread = new Thread(SendThreadLooperFunc);
            sendThread.Start();
            return true;
        }

        private void ReceiveThreadLooperFunc()
        {
            m_ReceivedHeader = new Byte[NetPacket.PACK_HEAD_SIZE];

            while (m_Socket != null && m_NeedClose == false)
            {
                bool b = ReadPacketHeader();
                if (b == false)
                {
                    m_NeedClose = true;
                    break;
                }
                b = ReadPacketBody();
                if (b == false)
                {
                    m_NeedClose = true;
                    break;
                }
            }
        }

        private bool ReadPacketHeader()
        {
            if (m_Socket == null || m_Socket.Connected == false)
            {
                return false;
            }
            try
            {
                for (int i = 0; i < m_ReceivedHeader.Length; i++)
                {
                    m_ReceivedHeader[i] = 0;
                }

                int receiveSize = m_Socket.Receive(m_ReceivedHeader, NetPacket.PACK_HEAD_SIZE, SocketFlags.None);
                if (receiveSize == 0)
                {
                    return false;
                }

                while (receiveSize < NetPacket.PACK_HEAD_SIZE)
                {
                    if (m_Socket == null || m_Socket.Connected == false)
                    {
                        return false;
                    }

                    int sendCount = m_Socket.Receive(m_ReceivedHeader, receiveSize, NetPacket.PACK_HEAD_SIZE - receiveSize, SocketFlags.None);

                    if (sendCount == 0)
                    {
                        return false;
                    }

                    receiveSize += sendCount;
                }

                if (NetPacket.IsPacketHeader(m_ReceivedHeader) == false)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool ReadPacketBody()
        {
            if (m_ReceivedHeader == null)
            {
                return false;
            }

            int msgID = BitConverter.ToInt32(m_ReceivedHeader, NetPacket.PACK_MESSAGEID_OFFSET);
            int bufferSize = BitConverter.ToInt32(m_ReceivedHeader, NetPacket.PACK_LENGTH_OFFSET);
            int bodySize = bufferSize - NetPacket.PACK_HEAD_SIZE;
            if (bodySize == 0)
            {
                return true;
            }
            NetPacket packet = new NetPacket(msgID, bodySize);

            if (packet.SetPacketHeader(m_ReceivedHeader) == false)
            {
                return false;
            }
            Byte[] packetBuffer = packet.GetBuffer();

            try
            {
                if (m_Socket == null || m_Socket.Connected == false)
                {
                    return false;
                }

                int receiveSize = m_Socket.Receive(packetBuffer, NetPacket.PACK_HEAD_SIZE, bodySize, SocketFlags.None);
                if (receiveSize == 0)
                {
                    return false;
                }

                while (receiveSize < bodySize)
                {
                    if (m_Socket == null || m_Socket.Connected == false)
                    {
                        return false;
                    }

                    int sendCount = m_Socket.Receive(packetBuffer, receiveSize + NetPacket.PACK_HEAD_SIZE, bodySize - receiveSize, SocketFlags.None);

                    if (sendCount == 0)
                    {
                        return false;
                    }
                    receiveSize += sendCount;
                }

                lock (m_ReceiveObject)
                {
                    m_ReceivedPackets.AddLast(packet);
                }
                return true;
            }
            catch (Exception e)
            {
                DoClose();
                return false;
            }
        }

        public bool SendMessage(int msgID, int userData, Byte[] data)
        {
            bool needCode = CodeMessageSet.bNeedCode(msgID);
            int bodySize = 0;
            if (data != null)
            {
                bodySize = data.Length;
            }
            Byte[] newData = data;

            if (needCode)
            {
                UInt32 crc = CodeMessageSet.CheckAndGetCodeNum(data);
                byte[] bytes = BitConverter.GetBytes(crc);
                bodySize += bytes.Length;
                newData = new Byte[bodySize];
                Array.Copy(data, 0, newData, 0, data.Length);
                Array.Copy(bytes, 0, newData, data.Length, bytes.Length);
            }

            NetPacket packet = new NetPacket(msgID, bodySize, needCode);
            bool b = packet.SetUserData(userData);
            if (b == false) return false;

            packet.SetData(newData);

            if (m_SendObject == null)
            {
                m_SendObject = new System.Object();
            }

            if (m_WillBeSentPackets == null)
            {
                m_WillBeSentPackets = new LinkedList<NetPacket>();
            }

            lock (m_SendObject)
            {
                m_WillBeSentPackets.AddLast(packet);
                m_SendEvent.Set();
            }
            return true;
        }

        public void GetAllReveivePackets(out List<NetPacket> packets)
        {
            packets = new List<NetPacket>();
            if (m_ReceiveObject != null)
            {
                lock (m_ReceiveObject)
                {
                    if (m_ReceivedPackets.Count == 0)
                    {
                        return;
                    }
                    packets.AddRange(m_ReceivedPackets);
                    m_ReceivedPackets.Clear();
                    return;
                }
            }
            return;
        }

        public NetPacket GetReceivePacket()
        {
            if (m_ReceiveObject == null)
            {
                return null;
            }
            lock (m_ReceiveObject)
            {
                if (m_ReceivedPackets.Count == 0)
                {
                    return null;
                }

                NetPacket packet = m_ReceivedPackets.First.Value;
                m_ReceivedPackets.RemoveFirst();
                return packet;
            }
        }

        private void SendThreadLooperFunc()
        {
            while (m_NeedClose == false && m_IsConnected == true && m_Socket != null)
            {
                NetPacket packet = null;

                if (m_SendObject == null)
                {
                    m_SendObject = new System.Object();
                }
                if (m_WillBeSentPackets == null)
                {
                    m_WillBeSentPackets = new LinkedList<NetPacket>();
                }

                lock (m_SendObject)
                {
                    if (m_WillBeSentPackets.Count != 0)
                    {
                        packet = m_WillBeSentPackets.First.Value;
                        m_WillBeSentPackets.RemoveFirst();
                    }
                }

                if (packet != null)
                {
                    try
                    {
                        Byte[] buffer = packet.GetBuffer();
                        int length = buffer.Length;
                        int sendSize = m_Socket.Send(buffer, 0, length, SocketFlags.None);
                        if (sendSize != length)
                        {
                            m_NeedClose = true;
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        m_NeedClose = true;
                        break;
                    }
                }
                else
                {
                    m_SendEvent.Reset();
                    m_SendEvent.WaitOne();
                }
            }
        }
    }
}
