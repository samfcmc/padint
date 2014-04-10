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
            oldValues.Add(timestamp, new Dictionary<int,int>());
            oldWriteTimestamps.Add(timestamp, new Dictionary<int, long>());
        }

        public void RemoveLogEntry(long timestamp)
        {
            oldValues.Remove(timestamp);
            oldWriteTimestamps.Remove(timestamp);
        }

        public void StorePadInt(long timestamp, int uid, int val, long writeTimestamp)
        {
            if (!oldValues[timestamp].Keys.Contains(uid))
            {
                oldValues[timestamp].Add(uid, val);
                oldWriteTimestamps[timestamp].Add(uid, writeTimestamp);
            }
        }

        public void RestorePadInt(PadInt p, long timestamp, int uid)
        {
            if (p.writeTimestamp > oldWriteTimestamps[timestamp][uid])
            {
                p.value = oldValues[timestamp][uid];
                p.writeTimestamp = oldWriteTimestamps[timestamp][uid];
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
                return oldWriteTimestamps[timestamp][uid];
            }
            return 0;
        }

        public int GetOldValue(int uid)
        {
            foreach (long timestamp in oldValues.Keys)
            {
                return oldValues[timestamp][uid];
            }
            return 0;
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
            string port = Console.ReadLine();

            TcpChannel channel = new TcpChannel(Convert.ToInt32(port));
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(RemoteDataServer),
                "RemoteDataServer",
                WellKnownObjectMode.Singleton);

            Console.WriteLine("Master Server Port");
            string master_port = Console.ReadLine();
            
            Console.WriteLine("Master Server address");
            string master_hostname = Console.ReadLine();

            RemoteDataServer.master = (IMasterServer)Activator.GetObject(
                typeof(IMasterServer),
                "tcp://" + master_hostname + ":" + master_port + "/RemoteMasterServer");

            RemoteDataServer.myUrl = "tcp://" + Dns.GetHostName() + ":" + port + "/RemoteDataServer";
            bool reg = RemoteDataServer.master.RegisterDataServer(RemoteDataServer.myUrl);
            
            if(reg)
            {
                Console.WriteLine("Connected to MasterServer");
            }
            
            Console.ReadLine();
        }
    }

    public class RemoteDataServer : MarshalByRefObject, IDataServer
    {
        Dictionary<int, PadInt> padInts = new Dictionary<int, PadInt>();
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
            if (!joinedTx.Contains(timestamp))
            {
                master.TxJoin(myUrl, timestamp);
                localTransactions.Add(timestamp, master.getTransaction(timestamp));
                joinedTx.Add(timestamp);
                log.AddLogEntry(timestamp);
            }
            if (timestamp < padInts[uid].readTimestamp || timestamp < padInts[uid].writeTimestamp)
            {
                master.TxAbort(timestamp);
                throw new Exception("TransactionAbortedException");
            }
            else
            {
                log.StorePadInt(timestamp, uid, padInts[uid].value, padInts[uid].writeTimestamp);

                padInts[uid].writeTimestamp = timestamp;
                padInts[uid].value = newvalue;
            }
        }

        public int Read(int uid, long timestamp)
        {
            if (!joinedTx.Contains(timestamp))
            {
                master.TxJoin(myUrl, timestamp);
                localTransactions.Add(timestamp, master.getTransaction(timestamp));
                joinedTx.Add(timestamp);
                log.AddLogEntry(timestamp);
            }
            if (padInts[uid].writeTimestamp > timestamp)
            {
                master.TxAbort(timestamp);
                throw new Exception("TransactionAbortedException");
            }
            else
            {
                long tempTimestamp = padInts[uid].writeTimestamp;
                int tempValue = padInts[uid].value;
                if (log.ContainsPadInt(uid))
                {
                    tempValue = log.GetOldValue(uid);
                    tempTimestamp = log.GetOldTimestamp(uid);
                }
                log.StorePadInt(timestamp, uid, tempValue, tempTimestamp);

                if (padInts[uid].writeTimestamp > 0)
                {
                    localTransactions[timestamp].dependencies.Add(padInts[uid].writeTimestamp);
                }
                padInts[uid].readTimestamp = Math.Max(timestamp, padInts[uid].readTimestamp);
                return padInts[uid].value;
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
            foreach(string s in metadata.servers) 
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
