using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clienteccharp
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientManejadorDeDatosRed.IniciadorPaquetesRed();
            ClientTCP.ConnectToServer();
            Console.ReadLine();
        }
    }
}
