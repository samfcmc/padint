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
        private Dictionary<string, IDataServer> dataServers = new Dictionary<string, IDataServer>();

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

        public PadIntMetadata CreatePadInt(int uid)
        {
            if (dataServers.Count > 0)
            {
                List<string> servers = getServersToStore();
                Console.WriteLine(servers);
                return new PadIntMetadata(uid, servers);
            }
            return null;
        }

        private List<string> getServersToStore() 
        {
            int rnd = new Random().Next(0, dataServers.Count);
            int secondServer = (rnd + 1) % dataServers.Count;
            Console.WriteLine("Server to store padInt index: " + rnd);
            string firstServerUrl = dataServers.Keys.ElementAt<string>(rnd);
            string secondServerUrl = dataServers.Keys.ElementAt<string>(secondServer);

            List<string> urls = new List<string>();
            urls.Add(firstServerUrl);

            if (!firstServerUrl.Equals(secondServerUrl))
            {
                urls.Add(secondServerUrl);
            }

            return urls;
        }

        public PadIntMetadata AccessPadInt(int uid)
        {
            return null;
        }

        public bool RegisterDataServer(string url)
        {
            if (dataServers.ContainsKey(url))
            {
                return false;
            }

            IDataServer remoteServer = (IDataServer)Activator.GetObject(
                typeof(IDataServer),
                url);
            dataServers.Add(url, remoteServer);
            Console.WriteLine("Server with url " + url + " is now part of the deep web.");
            return true;
        }
    }
}
