using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using conexion;


namespace clienteccharp
{
    class ClientTCP
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private byte[] _asyncbuffer = new byte[1204];

        public static void ConnectToServer()
        {
            Console.WriteLine("Conectandose al servidor...");
            _clientSocket.BeginConnect("127.0.0.1", 5555, new AsyncCallback(ConnectCallback), _clientSocket);
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            _clientSocket.EndConnect(ar);

            while(true)
            {
                //escuchando la informacion del servidor
                OnReceive();
            }
        }

        private static void OnReceive()
        {
            byte[] _sizeinfo = new byte[4];
            byte[] _recivedbuffer = new byte[1024];

            int totalLeido = 0, actualLeido = 0;

            try
            {
                actualLeido = totalLeido = _clientSocket.Receive(_sizeinfo);

                if(totalLeido <= 0)
                {
                    Console.WriteLine("No esta conectado al servidor");

                }
                else
                {
                    while(totalLeido < _sizeinfo.Length && actualLeido > 0)
                    {
                        actualLeido = _clientSocket.Receive(_sizeinfo, totalLeido, _sizeinfo.Length - totalLeido, SocketFlags.None);
                        totalLeido += actualLeido;
                    }

                    int messagesize = 0;
                    messagesize = _sizeinfo[0];
                    messagesize = (_sizeinfo[1] << 8);
                    messagesize = (_sizeinfo[2] << 16);
                    messagesize = (_sizeinfo[3] << 24);

                    byte[] data = new byte[messagesize];

                    totalLeido = 0;
                    actualLeido = totalLeido = _clientSocket.Receive(data, totalLeido, data.Length - totalLeido, SocketFlags.None);
                    while (totalLeido < messagesize && actualLeido > 0)
                    {
                        actualLeido = _clientSocket.Receive(data, totalLeido, data.Length - totalLeido, SocketFlags.None);
                        totalLeido += actualLeido;

                    }

                    //manejar la informacion de la red
                    ClientManejadorDeDatosRed.manejadorInfoDeRed(data);

                  }
            }
            catch (Exception e)
            {
                Console.WriteLine("No esta conectado al servidor...");
                Console.WriteLine(e);
            }
        }

        public static void SendData(byte[] data)
        {
            _clientSocket.Send(data);
        }

        public static void ThankYouServer()
        {
            packetbuffer buffer = new packetbuffer();
            buffer.escrbirEntero((int)ClientPackets.CThankyou);
            buffer.escrbirString("gracias por dejarme conectar al servidor");
            SendData(buffer.ToArray());
            buffer.Dispose();

        }
    }
}
