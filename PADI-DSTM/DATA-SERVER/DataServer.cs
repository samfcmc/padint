using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;

namespace PADI_DSTM
{
    public class Log
    {
        Dictionary<long, Dictionary<int, int>> oldValues = new Dictionary<long, Dictionary<int, int>>();
        Dictionary<long, Dictionary<int, long>> oldWriteTimestamps = new Dictionary<long, Dictionary<int, long>>();

        public void AddLogEntry(long timestamp)
        {
            oldValues.Add(timestamp, new Dictionary<int, int>());
            oldWriteTimestamps.Add(timestamp, new Dictionary<int, long>());
            Console.WriteLine("Add log entry to timestamp: " + timestamp);
        }

        public void RemoveLogEntry(long timestamp)
        {
            oldValues.Remove(timestamp);
            oldWriteTimestamps.Remove(timestamp);
            Console.WriteLine("Removed log entry from timestamp: " + timestamp);
        }

        public void StorePadInt(long timestamp, int uid, int val, long writeTimestamp)
        {
            if (!oldValues[timestamp].Keys.Contains(uid))
            {
                oldValues[timestamp].Add(uid, val);
                oldWriteTimestamps[timestamp].Add(uid, writeTimestamp);
                Console.WriteLine("Stored padint: " + uid + " with val: " + val + "and writetimestamp: " + writeTimestamp + " to timestamp: " + timestamp);
            }
        }

        public void RestorePadInt(PadInt p, long timestamp, int uid)
        {
            if (p.writeTimestamp > oldWriteTimestamps[timestamp][uid])
            {
                p.value = oldValues[timestamp][uid];
                p.writeTimestamp = oldWriteTimestamps[timestamp][uid];
                Console.WriteLine("Restored padint: " + uid + " to val: " + p.value + "and writetimestamp: " + p.writeTimestamp + " in timestamp: " + timestamp);
            }
        }

        public Dictionary<int, int> GetTimestampUIDs(long timestamp)
        {
            return oldValues[timestamp];
        }

        public long GetOldTimestamp(int uid)
        {
            foreach (long timestamp in oldWriteTimestamps.Keys)
            {
                /* Protects against uninitialized entries */
                if (oldWriteTimestamps[timestamp].Count > 0)
                {
                    Console.WriteLine("Got old timstamp of padint: " + uid + " in timestamp: " + timestamp);
                    return oldWriteTimestamps[timestamp][uid];
                }
            }
            return -1;
        }

        public int GetOldValue(int uid)
        {
            foreach (long timestamp in oldValues.Keys)
            {
                /* Protects against uninitialized entries */
                if (oldValues[timestamp].Count > 0)
                {
                    Console.WriteLine("Got old value of padint: " + uid + " in timestamp: " + timestamp);
                    return oldValues[timestamp][uid];
                }
            }
            return -1;
        }

        public bool Contains(long timestamp)
        {
            return oldValues.Keys.Contains(timestamp);
        }

        public bool ContainsPadInt(int uid)
        {
            foreach (long timestamp in oldWriteTimestamps.Keys)
            {
                if (oldWriteTimestamps[timestamp].Keys.Contains(uid))
                {
                    Console.WriteLine("Found padint: " + uid + " in timestamp: " + timestamp);
                    return true;
                }
            }
            return false;
        }
    }

    class DataServer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server port");
            string port = "2001";// Console.ReadLine();

            TcpChannel channel = new TcpChannel(Convert.ToInt32(port));
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(RemoteDataServer),
                "RemoteDataServer",
                WellKnownObjectMode.Singleton);

            Console.WriteLine("Master Server Port");
            string master_port = "8086"; //Console.ReadLine();

            Console.WriteLine("Master Server address");
            string master_hostname = "localhost"; //Console.ReadLine();

            RemoteDataServer.master = (IMasterServer)Activator.GetObject(
                typeof(IMasterServer),
                "tcp://" + master_hostname + ":" + master_port + "/RemoteMasterServer");

            RemoteDataServer.myUrl = "tcp://" + Dns.GetHostName() + ":" + port + "/RemoteDataServer";
            bool reg = RemoteDataServer.master.RegisterDataServer(RemoteDataServer.myUrl);

            if (reg)
            {
                Console.WriteLine("Connected to MasterServer");
            }

            Console.ReadLine();
        }
    }

    public class RemoteDataServer : MarshalByRefObject, IDataServer
    {
        Dictionary<int, PadInt> padInts = new Dictionary<int, PadInt>();
        /// <summary>
        /// Stores the dependencies of all transactions in the server. A 
        /// transaction can only depend on another transaction, if it reads 
        /// the write of another transaction in the same padint in the same 
        /// server.
        /// </summary>
        Dictionary<long, PadiTransaction> localTransactions = new Dictionary<long, PadiTransaction>();
        Log log = new Log();
        List<long> joinedTx = new List<long>();
        public static string myUrl;
        public static IMasterServer master;

        public List<long> getTxDependencies(long timestamp)
        {
            return localTransactions[timestamp].dependencies;
        }

        public void Write(int uid, long timestamp, int newvalue)
        {
            PadInt padint = padInts[uid];
            if (!joinedTx.Contains(timestamp))
            {
                master.TxJoin(myUrl, timestamp);
                localTransactions.Add(timestamp, master.getTransaction(timestamp));
                joinedTx.Add(timestamp);
                log.AddLogEntry(timestamp);
            }
            if (timestamp < padint.readTimestamp || timestamp < padint.writeTimestamp)
            {
                master.TxAbort(timestamp);
                throw new Exception("TransactionAbortedException");
            }
            else
            {
                log.StorePadInt(timestamp, uid, padint.value, padint.writeTimestamp);

                padint.writeTimestamp = timestamp;
                padint.value = newvalue;
            }
        }

        public int Read(int uid, long timestamp)
        {
            PadInt padint = padInts[uid];
            if (!joinedTx.Contains(timestamp))
            {
                master.TxJoin(myUrl, timestamp);
                localTransactions.Add(timestamp, master.getTransaction(timestamp));
                joinedTx.Add(timestamp);
                log.AddLogEntry(timestamp);
            }
            if (padint.writeTimestamp > timestamp)
            {
                master.TxAbort(timestamp);
                throw new Exception("TransactionAbortedException");
            }
            else
            {
                long oldWriteTimestamp = padint.writeTimestamp;
                int oldValue = padint.value;
                if (log.ContainsPadInt(uid))
                {
                    oldValue = log.GetOldValue(uid);
                    oldWriteTimestamp = log.GetOldTimestamp(uid);
                }
                log.StorePadInt(timestamp, uid, oldValue, oldWriteTimestamp);

                if (padint.writeTimestamp > 0 && padint.writeTimestamp != timestamp)
                {
                    localTransactions[timestamp].dependencies.Add(padint.writeTimestamp);
                }
                padint.readTimestamp = Math.Max(timestamp, padint.readTimestamp);
                return padint.value;
            }
        }

        public bool TxPrepare(long timestamp)
        {
            return true;
        }

        public bool TxCommit(long timestamp)
        {
            log.RemoveLogEntry(timestamp);
            return joinedTx.Remove(timestamp);
        }

        public bool TxAbort(long timestamp)
        {
            if (log.Contains(timestamp))
            {
                foreach (int uid in log.GetTimestampUIDs(timestamp).Keys)
                {
                    log.RestorePadInt(padInts[uid], timestamp, uid);
                }
                log.RemoveLogEntry(timestamp);
                //localTransactions.Remove(timestamp);
                joinedTx.Remove(timestamp);
                return true;
            }
            else
            {
                //localTransactions.Remove(timestamp);
                joinedTx.Remove(timestamp);
                return true;
            }
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
            foreach (IChannel channel in ChannelServices.RegisteredChannels)
            {
                ChannelServices.UnregisterChannel(channel);
            }
            Environment.Exit(0);
        }

        public bool Freeze()
        {
            Monitor.Enter(this);
            return true;
        }

        public bool Recover()
        {
            Monitor.PulseAll(this);
            return true;
        }

        public PadInt CreatePadInt(int uid, PadIntMetadata metadata)
        {
            if (padInts.ContainsKey(uid))
            {
                return null;
            }
            PadInt p = new PadInt(uid);
            foreach (string s in metadata.servers)
            {
                p.servers.Add(s);
            }
            padInts.Add(uid, p);
            Console.WriteLine("Created PadInt with uid: " + uid);
            return p;
        }

        public PadInt AccessPadInt(int uid)
        {
            return padInts[uid];
        }

    }
}
