using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PADI_DSTM
{

    class MasterServer
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(RemoteMasterServer),
                "RemoteMasterServer",
                WellKnownObjectMode.Singleton);

            Console.WriteLine("<enter> to continue");
            Console.ReadLine();
        }
    }

    public class RemoteMasterServer : MarshalByRefObject, IMasterServer
    {
        private Dictionary<string, IServer> dataServers = new Dictionary<string, IServer>();

        private Dictionary<uint, Transaction> transactions = new Dictionary<uint, Transaction>();
        private uint txIdCount = 0;

        public bool Init()
        {
            return true;
        }

        public bool TxBegin()
        {
            txIdCount++;
            transactions.Add(txIdCount, new Transaction(txIdCount));
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

        public bool RegisterDataServer(string url)
        {
            IServer remoteServer = (IServer)Activator.GetObject(
                typeof(IServer),
                url);
            dataServers.Add(url, remoteServer);
            Console.WriteLine("Server with url " + url + " is now part of the deep web.");
            return true;
        }
    }
}
