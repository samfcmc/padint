using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PADI_DSTM
{

    public class MasterServer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Master port");
            string portString = Console.ReadLine();
            int port = Convert.ToInt32(portString);

            launchMasterServer(port);

            Console.ReadLine();
        }

        public static void launchMasterServer(int port)
        {
            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(RemoteMasterServer),
                "RemoteMasterServer",
                WellKnownObjectMode.Singleton);

            Console.WriteLine("MasterServer Running...");
        }
    }

    
}
