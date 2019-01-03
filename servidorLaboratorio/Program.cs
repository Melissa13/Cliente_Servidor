using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace servidorLaboratorio
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerHandleNetworkData.IniciadorPaquetesRed();
            ServerTCP.SetupServer();
            Console.ReadLine();
        }
    }
}
