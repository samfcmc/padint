using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace PADI_DSTM
{
    public class Heartbeat
    {
        public string nextServer;

        public void Ping(Object stateInfo)
        {
            try
            {
                IDataServer next = (IDataServer)Activator.GetObject(
                    typeof(IDataServer),
                    nextServer);
                next.Echo();
                Console.WriteLine("pinged: {0}", nextServer);
            }
            catch (SocketException e)
            {
                //TODO: master logic goes here
                Console.WriteLine("Server with url {0} is not responding.", nextServer);
            }
        }
    }
}
