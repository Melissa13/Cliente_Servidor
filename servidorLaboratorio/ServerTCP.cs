using System;

using System.Net.Sockets;
using System.Net;
using conexion;

namespace servidorLaboratorio
{
    class ServerTCP
    {
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //buffer que manejara la data
        private static byte[] _buffer = new byte[1024];

        //creando instancias del cliente
        public static Client[] _clientes = new Client[Constante.MAX_USERS];

        public static void SetupServer()
        {
            for(int i = 0; i< Constante.MAX_USERS; i++)
            {
                _clientes[i] = new Client();
            }
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 5555));
            _serverSocket.Listen(10);//cantidad de usuarios que se pueden conectar
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = _serverSocket.EndAccept(ar);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

            for(int i = 0; i < Constante.MAX_USERS; i++)
            {
                if (_clientes[i].socket == null)
                {
                    _clientes[i]. socket = socket;
                    _clientes[i].index = i;
                    _clientes[i].ip = socket.RemoteEndPoint.ToString();
                    _clientes[i].StartClient();
                    Console.WriteLine("conexion de '{0}' recivida", _clientes[i].ip);
                    SendConnectionOk(i);
                    return;
                }
            }
        }

        //enviar data por la red
        public static void SendDataTo(int index, byte[] data)
        {
            byte[] sizeinfo = new byte[4];
            sizeinfo[0] = (byte)data.Length;
            sizeinfo[1] = (byte)(data.Length >> 8);
            sizeinfo[2] = (byte)(data.Length >> 16);
            sizeinfo[3] = (byte)(data.Length >> 24);

            _clientes[index].socket.Send(sizeinfo); //tiempo de espera
            _clientes[index].socket.Send(data);

        }

        public static void SendConnectionOk(int index)
        {
            packetbuffer buffer = new packetbuffer();
            buffer.escrbirEntero((int)ServerPackets.SconnectionOk);
            buffer.escrbirString("coneccion exitosa con el servidor");
            SendDataTo(index, buffer.ToArray());
            buffer.Dispose();
        }
    }


    class Client
    {
        //instancia creada para los usuarios que se logueen
        public int index;
        public string ip;
        public Socket socket;
        public bool closing = false;
        private byte[] _buffer = new byte[1024];

        public void StartClient()
        {
            //recibiendo data del cliente
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            closing = false;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            
            try
            {
                int received = socket.EndReceive(ar);

                if(received <= 0)
                {
                    CloseClient(index);
                }
                else
                {
                    byte[] databuffer = new byte[received];
                    Array.Copy(_buffer, databuffer, received);
                    //manejar la informacion de la red
                    ServerHandleNetworkData.manejadorInfoDeRed(index, databuffer);
                    socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                }
            }
            catch (Exception e)
            {
                CloseClient(index);
                Console.WriteLine(e);
            }
        }

        private void CloseClient(int index)
        {
            closing = true;
            Console.WriteLine("coneccion de {0} ha terminado", ip);
            //el usuario ha dejado el servidor
            socket.Close();
        }
    }
}
