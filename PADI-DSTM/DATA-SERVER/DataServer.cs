using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PADI_DSTM
{
    class DataServer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Where is mah port?");
            string port = Console.ReadLine();

            TcpChannel channel = new TcpChannel(Convert.ToInt32(port));
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(RemoteDataServer),
                "RemoteDataServer",
                WellKnownObjectMode.Singleton);

            Console.WriteLine("What is the master port?");
            string master_port = Console.ReadLine();
            
            Console.WriteLine("What is the master hostname?");
            string master_hostname = Console.ReadLine();

            IMasterServer master = (IMasterServer)Activator.GetObject(
                typeof(IMasterServer),
                "tcp://" + master_hostname + ":" + master_port + "/RemoteMasterServer");

            master.RegisterDataServer("tcp://" + Dns.GetHostName() + ":" + port + "/RemoteDataServer");
            if (master.Status())
            {
                Console.WriteLine("Master is up!");
            }
            else Console.WriteLine("You are weird!");
            

            Console.WriteLine("<enter> to continue");
            Console.ReadLine();
        }
    }

    public class RemoteDataServer : MarshalByRefObject, IServer
    {
        public bool Init()
        {
            return true;
        }

        public bool TxBegin()
        {
            return false;
        }

        public bool TxCommit()
        {
            return false;
        }

        public bool TxAbort()
        {
            return false;
        }

        public bool Status()
        {
            Console.WriteLine("Printing Status");
            Console.WriteLine("---------------");
            return true;
        }

        public bool Fail(string url)
        {
            return false;
        }

        public bool Freeze(string url)
        {
            return false;
        }

        public bool Recover(string url)
        {
            return false;
        }

        public PadInt CreatePadInt(int uid)
        {
            return new PadInt();
        }

        public PadInt AccessPadInt(int uid)
        {
            return null;
        }

    }
}
