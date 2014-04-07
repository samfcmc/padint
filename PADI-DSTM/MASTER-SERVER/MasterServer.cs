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

            Console.WriteLine("MasterServer Running...");
            Console.ReadLine();
        }
    }

    public class RemoteMasterServer : MarshalByRefObject, IMasterServer
    {
        private Dictionary<string, IDataServer> dataServers = new Dictionary<string, IDataServer>();
        private Dictionary<int, PadIntMetadata> metadata = new Dictionary<int, PadIntMetadata>();

        //private Dictionary<uint, Transaction> transactions = new Dictionary<uint, Transaction>();
        private uint txIdCount = 0;

        public Dictionary<string, IDataServer> getDataServers()
        {
            return dataServers;
        }

        public bool TxBegin()
        {
            /*txIdCount++;
            transactions.Add(txIdCount, new Transaction(txIdCount));*/
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
            Console.WriteLine("Up and Running!");
            return true;
        }

        public void Fail()
        {
        }

        public bool Freeze()
        {
            return false;
        }

        public bool Recover()
        {
            return false;
        }

        public PadIntMetadata CreatePadInt(int uid)
        {
            if (dataServers.Count > 0)
            {
                if (metadata.ContainsKey(uid))
                {
                    return null;
                }
                List<string> servers = getServersToStore();
                PadIntMetadata pmeta = new PadIntMetadata();
                pmeta.uid = uid;
                pmeta.servers = servers;
                metadata.Add(uid, pmeta);
                return pmeta;
            }
            return null;
        }

        private List<string> getServersToStore() 
        {
            int count = dataServers.Count;
            List<string> urls = new List<string>();

            int firstServer = new Random().Next(0, count);
            string firstServerUrl = dataServers.Keys.ElementAt<string>(firstServer);
            urls.Add(firstServerUrl);
            Console.WriteLine("First Server to store padInt: " + firstServerUrl);

            if (count > 1)
            {
                int secondServer = (firstServer + 1) % count;
                string secondServerUrl = dataServers.Keys.ElementAt<string>(secondServer);
                urls.Add(secondServerUrl);
                Console.WriteLine("Second Server to store padInt: " + secondServerUrl);
            }

            return urls;
        }

        public PadIntMetadata AccessPadInt(int uid)
        {
            return metadata[uid];
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
            Console.WriteLine("Server with url " + url + " is now connected.");
            return true;
        }
    }
}
