using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM
{
    public class RemoteMasterServer : MarshalByRefObject, IMasterServer
    {
        private Dictionary<string, IDataServer> dataServers = new Dictionary<string, IDataServer>();
        private Dictionary<string, string> nextServers = new Dictionary<string, string>();
        private Dictionary<int, PadIntMetadata> metadata = new Dictionary<int, PadIntMetadata>();

        private Dictionary<long, PadiTransaction> transactions = new Dictionary<long, PadiTransaction>();
        private long currentTimestamp = 1;

        public PadiTransaction getTransaction(long timestamp)
        {
            return transactions[timestamp];
        }

        public string GetNextServer(string dataServer)
        {
            return nextServers[dataServer];
        }

        public Dictionary<string, IDataServer> getDataServers()
        {
            return dataServers;
        }

        public long TxBegin()
        {
            PadiTransaction tx = new PadiTransaction(currentTimestamp);
            tx.state = STATE.RUNNING;
            transactions.Add(currentTimestamp, tx);
            return currentTimestamp++;
        }

        public bool TxJoin(string url, long timestamp)
        {
            if (transactions[timestamp].servers.Contains(url))
            {
                return false;
            }
            else transactions[timestamp].servers.Add(url);
            return true;
        }

        public bool TxCommit(long timestamp)
        {
            List<IDataServer> servers = new List<IDataServer>();
            bool failed = false;

            foreach (string s in transactions[timestamp].servers)
            {
                IDataServer server = (IDataServer)Activator.GetObject(
                    typeof(IDataServer),
                    s);
                foreach (long l in server.GetTxDependencies(timestamp))
                {
                    if (transactions[l].state != STATE.COMMITTED)
                    {
                        failed = true;
                        break;
                    }
                }

                servers.Add(server);
                if (!server.TxPrepare(timestamp))
                {
                    failed = true;
                }
            }
            if (failed)
            {
                TxAbort(timestamp);
                return false;
            }

            foreach (IDataServer server in servers)
            {
                server.TxCommit(timestamp);
            }
            transactions[timestamp].state = STATE.COMMITTED;
            //transactions.Remove(timestamp);
            return true;
        }

        public bool TxAbort(long timestamp)
        {
            foreach (string s in transactions[timestamp].servers)
            {
                IDataServer server = (IDataServer)Activator.GetObject(
                    typeof(IDataServer),
                    s);
                server.TxAbort(timestamp);
            }
            transactions[timestamp].state = STATE.ABORTED;
            //transactions.Remove(timestamp);
            return true;
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
            if (metadata.ContainsKey(uid))
            {
                return metadata[uid];
            }
            else
            {
                return null;
            }

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

            if (nextServers.Count == 0)
            {
                remoteServer.SetNextServer(url);
            }
            else
            {
                string theOne = nextServers.Keys.ElementAt<string>(new Random().Next(0, nextServers.Keys.Count));
                remoteServer.SetNextServer(nextServers[theOne]);
                nextServers[theOne] = url;
                IDataServer theOneServer = (IDataServer)Activator.GetObject(
                    typeof(IDataServer),
                    theOne);
                theOneServer.SetNextServer(url);
            }
            nextServers.Add(url, remoteServer.GetNextServer());
            
            Console.WriteLine("Server with url " + url + " is now connected.");
            return true;
        }
    }
}
