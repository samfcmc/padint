using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using PADI_DSTM.Exceptions;
using System.Runtime.Serialization.Formatters;
using System.Collections;

namespace PADI_DSTM
{
    public class DataServer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server port");
            string portString = Console.ReadLine();
            int port = Convert.ToInt32(portString);

            Console.WriteLine("Master Server Port");
            string master_port = Console.ReadLine();

            Console.WriteLine("Master Server address");
            string master_hostname = Console.ReadLine();

            string masterUrl = "tcp://" + master_hostname + ":" + master_port + "/RemoteMasterServer";

            launchDataServer(port, masterUrl);

            Console.ReadLine();
        }

        public static void launchDataServer(int port, string masterUrl)
        {
            BinaryServerFormatterSinkProvider serverProv =
                new BinaryServerFormatterSinkProvider();
            serverProv.TypeFilterLevel = TypeFilterLevel.Full; 

            IDictionary propBag = new Hashtable();
            propBag["port"] = port;
            propBag["typeFilterLevel"] = TypeFilterLevel.Full;
            propBag["name"] = "tcp" + port;  // here enter unique channel name

            TcpChannel channel = new TcpChannel(propBag, null, serverProv);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(RemoteDataServer),
                "RemoteDataServer",
                WellKnownObjectMode.Singleton);

            RemoteDataServer.master = (IMasterServer)Activator.GetObject(
                typeof(IMasterServer),
                masterUrl);

            RemoteDataServer.myUrl = "tcp://" + Dns.GetHostName() + ":" + port + "/RemoteDataServer";
            bool reg = RemoteDataServer.master.RegisterDataServer(RemoteDataServer.myUrl);

            if (reg)
            {
                Console.WriteLine("Connected to MasterServer");
            }
        }
    }

    
}
