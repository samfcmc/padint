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
            Console.WriteLine("CLIENT APPLICATION v1.0");

            Console.WriteLine("What is the master port?");
            string master_port = Console.ReadLine();

            Console.WriteLine("What is the master hostname?");
            string master_hostname = Console.ReadLine();

            IMasterServer master = (IMasterServer)Activator.GetObject(
                typeof(IMasterServer),
                "tcp://" + master_hostname + ":" + master_port + "/RemoteMasterServer");

            master.Status();
            Console.ReadLine();
        }
    }
}
