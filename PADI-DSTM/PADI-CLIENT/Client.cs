using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM
{
    class Client
    {
        static void Main(string[] args)
        {
            RemoteMasterServer s = (RemoteMasterServer)Activator.GetObject(typeof(RemoteMasterServer), "tcp://localhost:8086/RemoteMasterServer");

            s.Status();
            Console.ReadLine();
        }
    }
}
