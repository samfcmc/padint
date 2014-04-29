using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace PADI_DSTM
{
    public class Heartbeat
    {
        private bool pingable = true;
        public string nextServer;

        public void Ping(Object stateInfo)
        {
            if (pingable)
            {
                pingable = false;
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
                    Console.WriteLine("Server with url {0} is not responding.", nextServer);
                    nextServer = RemoteDataServer.master.NotifyFault(RemoteDataServer.myUrl, nextServer);
                }
                pingable = true;
            }
        }
    }
}
