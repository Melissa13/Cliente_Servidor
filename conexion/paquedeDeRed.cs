using System;
using System.Collections.Generic;
using System.Text;

namespace conexion
{
    //se envia desde el servidor al cliente
    //el cliente tiene que escuchar el paquete del servidor
    public enum ServerPackets
    {
        SconnectionOk = 1, 
    }

    //se envia desd el cliente al servidor 
    //el servidor escucha el pedido del cliente
    public enum ClientPackets
    {
        CThankyou =1,
    }

}
