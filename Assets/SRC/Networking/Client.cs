using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using GameClient;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;

    public string localIp = "127.0.0.1";
    public string Ip = "127.0.0.1";
    public int port = 9600;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;
    [SerializeField] private bool isConnected;


    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;
    // * SINGELTON IMPLEMENTATION
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("SAMEINSTACE ");
            Destroy(this);
        }
    }
    private void Start()
    {
        tcp = new TCP();
        udp = new UDP();

    }
    private void OnApplicationQuit()
    {
        Disconnect();
    }
    public void ConnectToServer()
    {
        InitializeClientData();
        instance.Ip = localIp;
        isConnected = true;
        tcp.Connect();
    }
    public void ConnectToServer(string otherIp)
    {
        InitializeClientData();
        instance.Ip = otherIp;
        isConnected = true;
        tcp.Connect();
    }
    public class TCP
    {
        public TcpClient socket;
        private Packet reccievePacket;
        private NetworkStream stream;
        private byte[] recceiveBuffer;
        public void Connect()
        {
            Debug.Log(instance.Ip);
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            recceiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.Ip, instance.port, ConnectCallback, socket);
        }
        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);
            if (!socket.Connected)
                return;
            stream = socket.GetStream();
            reccievePacket = new Packet();
            stream.BeginRead(recceiveBuffer, 0, dataBufferSize, RecceiveCallback, null);
        }
        private void RecceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }
                byte[] _data = new byte[_byteLength];
                Array.Copy(recceiveBuffer, _data, _byteLength);

                reccievePacket.Reset(HandleData(_data));

                stream.BeginRead(recceiveBuffer, 0, dataBufferSize, RecceiveCallback, null);
            }
            catch (Exception _ex)
            {
                System.Console.WriteLine($"Error recciving TCP data {_ex}");
                Disconnect();
            }
        }
        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;
            reccievePacket.SetBytes(_data);
            if (reccievePacket.UnreadLength() >= 4)
            {
                _packetLength = reccievePacket.ReadInt();
                if (_packetLength <= 0)
                { return true; }
            }
            while (_packetLength > 0 && _packetLength <= reccievePacket.UnreadLength())
            {
                byte[] _packetBytes = reccievePacket.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        if (!packetHandlers.ContainsKey(_packetId))
                            Debug.Log("Key not found: " + _packetId);
                        packetHandlers[_packetId](_packet);
                    }
                });
                _packetLength = 0;
                if (reccievePacket.UnreadLength() >= 4)
                {
                    _packetLength = reccievePacket.ReadInt();
                    if (_packetLength <= 0)
                        return true;
                }
            }
            if (_packetLength <= 1)
            {
                return true;
            }
            return false;

        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.LogError($"Send Fail id: {instance.myId} ,Exception: {_ex} ");
                throw;
            }
        }
        private void Disconnect()
        {
            instance.Disconnect();
            stream = null;
            reccievePacket = null;
            recceiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;
        public UDP()
        {
            Debug.Log("IP: " + instance.Ip);
            endPoint = new IPEndPoint(IPAddress.Parse(instance.Ip), instance.port);
        }
        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);
            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);
            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }
        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.myId); //* "Typically only one UDP client on the server."
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }

            }
            catch (Exception _ex)
            {

                Debug.Log($"Exception during UDP send ex: {_ex}");
            }
        }
        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);
                if (_data.Length < 4)
                {
                    //TODO "Maby perform multiple checks before disconnecting" "if high network traffic"
                    instance.Disconnect();
                    return;
                }
                HandleData(_data);
            }
            catch (Exception _ex)
            {

                Debug.Log($"UDP exception during ReceiveCallback ex: {_ex}");
                Disconnect();
            }
        }
        private void HandleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data)) //Contains all 4096 bytes
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data)) //Shortened byte array
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet);
                }
            });
        }
        private void Disconnect()
        {
            instance.Disconnect();
            endPoint = null;
            socket = null;
        }

    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.welcome,ClientHandle.Welcome},
            {(int)ServerPackets.UDPConnect,ClientHandle.UDPConnect},
            {(int)ServerPackets.fen,ClientHandle.Fen},
            {(int)ServerPackets.move,ClientHandle.Move}
        };
        Debug.Log("Data Initialized");
    }
    private void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            NetworkUIManager.instance.connectedTCP = false;
            NetworkUIManager.instance.connectedUDP = false;
            tcp.socket.Close();
            udp.socket.Close();
            Debug.Log("Disconnected from server.");
            ConsoleHistory.instance.addLogHistory($"<color=red>     <b>Disconnected from server!</b> </color>");
            NetworkUIManager.instance.onDisconnect();

        }
    }
}
