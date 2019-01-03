﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using conexion;

namespace clienteccharp
{
    class ClientManejadorDeDatosRed
    {
        private delegate void Packet_(byte[]data);
        private static Dictionary<int, Packet_> Packets;

        public static void IniciadorPaquetesRed()
        {
            Console.WriteLine("iniciando la red de paquetes");
            Packets = new Dictionary<int, Packet_>
            {
                { (int)ServerPackets.SconnectionOk, ManejadorConexionOk}
            };
        }

        public static void manejadorInfoDeRed(byte[] data)
        {
            int numeroPaquete; packetbuffer buffer = new packetbuffer();
            buffer.escrbir(data);
            numeroPaquete = buffer.leerEntero();
            buffer.Dispose();
            //escepcion
            Packet_ Packet;
            if(Packets.TryGetValue(numeroPaquete,out Packet))
            {
                Packet.Invoke(data);
            }

        }

        private static void ManejadorConexionOk(byte[] data)
        {
            //sacar la informacion del paquete
            packetbuffer buffer = new packetbuffer();
            buffer.escrbir(data);
            //int number= buffer.leerEntero();
            //no se toma el valor porque no es necesario, en dicho caso solo se hace para seguir
            buffer.leerEntero();
            string msg = buffer.leerString();
            buffer.Dispose();

            //agregar el codigo a ejecutar aqui
            Console.WriteLine(msg);

            ClientTCP.ThankYouServer();
        }

    }
}
