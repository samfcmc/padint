using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM
{
    public class RemoteMasterServer : MarshalByRefObject, IMasterServer
    {
        private Dictionary<string, ServerMetadata> dataServers = new Dictionary<string, ServerMetadata>();
        private Dictionary<string, string> nextServers = new Dictionary<string, string>();
        private Dictionary<int, PadIntMetadata> metadata = new Dictionary<int, PadIntMetadata>();
        private List<PadIntMetadata> nonReplicatedPadInts = new List<PadIntMetadata>();

        private Dictionary<long, PadiTransaction> transactions = new Dictionary<long, PadiTransaction>();
        private long currentTimestamp = 1;
        private readonly object timestampLock = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="server">server to replicate to</param>
        /// <param name="serverURL">Server's url to replicate to</param>
        private void ReplicatePadInts(IDataServer server, String serverURL)
        {
            List<PadIntMetadata> toRemove = new List<PadIntMetadata>();
            foreach (PadIntMetadata meta in nonReplicatedPadInts)
            {
                IDataServer dataServer = (IDataServer)Activator.GetObject(
                        typeof(IDataServer),
                        meta.servers.ToArray()[0]); //there's only one element in the list after removing the faulty server
                PadInt p = dataServer.AccessPadInt(meta.uid);
                server.StorePadInt(meta.uid, p);
                dataServers[serverURL].PadintCount++;
                metadata[meta.uid].servers.Add(serverURL);
                toRemove.Add(meta);   
            }

            foreach(PadIntMetadata meta in toRemove) {
                nonReplicatedPadInts.Remove(meta);
            }
        }

        private void CheckForNonReplicatedPadInts()
        {
            foreach (PadIntMetadata meta in metadata.Values)
            {
                if (meta.servers.Count < 2)
                {
                    nonReplicatedPadInts.Add(meta);
                }
            }
        }

        private void RestorePadInts(string faultyServer)
        {
            dataServers.Remove(faultyServer);
            foreach (PadIntMetadata meta in metadata.Values)
            {
                if (meta.servers.Contains(faultyServer))
                {
                    meta.servers.Remove(faultyServer);
                    IDataServer dataServer = (IDataServer)Activator.GetObject(
                        typeof(IDataServer),
                        meta.servers.ToArray()[0]); //there's only one element in the list after removing the faulty server
                    PadInt p = dataServer.AccessPadInt(meta.uid);
                    p.servers.Remove(faultyServer);

                    if (dataServers.Count < 2)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (string server in dataServers.Keys)
                        {
                            if (!meta.servers.ToArray()[0].Equals(server))
                            {
                                p.servers.Add(server);
                                dataServers[server].RemoteObject.StorePadInt(meta.uid, p);
                                dataServers[server].PadintCount++;
                                meta.servers.Add(server);
                                break;
                            }
                        }
                        dataServers[meta.servers.ToArray()[0]].RemoteObject.StorePadInt(meta.uid, p);
                    }
                }
            }
        }

        public string NotifyFault(string notifier, string faultyServer)
        {
            RestorePadInts(faultyServer);
            CheckForNonReplicatedPadInts();

            string faultyNext = nextServers[faultyServer];
            nextServers.Remove(faultyServer);
            nextServers[notifier] = faultyNext;
            return faultyNext;
        }

        public PadiTransaction getTransaction(long timestamp)
        {
            return transactions[timestamp];
        }

        public string GetNextServer(string dataServer)
        {
            return nextServers[dataServer];
        }

        public Dictionary<string, ServerMetadata> getDataServers()
        {
            return dataServers;
        }

        public long TxBegin()
        {
            long timestamp;
            lock (timestampLock)
            {
                timestamp = currentTimestamp++;
            }
            PadiTransaction tx = new PadiTransaction(timestamp);
            tx.state = STATE.RUNNING;
            transactions.Add(timestamp, tx);
            return timestamp;
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
                foreach(string url in servers) {
                    dataServers[url].PadintCount++;
                }
                if (servers.Count < 2)
                {
                    nonReplicatedPadInts.Add(pmeta);
                }

                return pmeta;
            }
            return null;
        }
        
        private List<string> getServersToStore()
        {
            int count = dataServers.Count;
            List<string> urls = new List<string>();
            ICollection<ServerMetadata> servers = dataServers.Values;
            var orderedServers = from s in servers
                      orderby s.PadintCount
                      select s;
            ServerMetadata[] smetasArray = orderedServers.ToArray<ServerMetadata>();

            string firstServerUrl = smetasArray[0].Url;
            urls.Add(firstServerUrl);
            Console.WriteLine("First Server to store padInt: " + firstServerUrl);

            if (count > 1)
            {
                string secondServerUrl = smetasArray[1].Url;
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
            ServerMetadata smeta = new ServerMetadata(remoteServer, url);
            dataServers.Add(url, smeta);

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

            ReplicatePadInts(remoteServer,url);

            Console.WriteLine("Server with url " + url + " is now connected.");
            return true;
        }
    }
}
