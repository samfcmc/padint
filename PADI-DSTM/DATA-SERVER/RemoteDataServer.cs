using PADI_DSTM.Exceptions;
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
        List<int> usedPadInts = new List<int>();
        public static string myUrl;
        public static IMasterServer master;
        private object locker = new object();

        /// <summary>
        /// The next server on the ring of servers to ping, to detect faults.
        /// </summary>
        public static Heartbeat heartbeat = new Heartbeat();

        bool frozen = false;
        object monitor = new object();

        public string GetURL()
        {
            return myUrl;
        }

        public void StorePadInt(int uid, PadInt p)
        {
            if (!padInts.Keys.Contains(uid))
            {
                p.servers.Add(myUrl);
                padInts.Add(uid, p);
                Console.WriteLine("Re-Stored PadInt with uid {0}.", uid);
            }
            else
            {
                padInts[uid] = p;
            }
        }

        public bool Echo()
        {
            return true;
        }

        public string GetNextServer()
        {
            return heartbeat.nextServer;
        }

        public void SetNextServer(string next)
        {
            heartbeat.nextServer = next;
        }

        public List<long> GetTxDependencies(long timestamp)
        {
            return localTransactions[timestamp].dependencies;
        }

        public void Write(int uid, long timestamp, int newvalue)
        {
            checkFreeze();

            PadInt padint = padInts[uid];
            if (!joinedTx.Contains(timestamp))
            {
                master.TxJoin(myUrl, timestamp);
                lock (locker)
                {
                    localTransactions.Add(timestamp, master.getTransaction(timestamp));
                    joinedTx.Add(timestamp);
                }
            }
            if (timestamp < padint.readTimestamp || timestamp < padint.writeTimestamp)
            {
                master.TxAbort(timestamp);
                throw new TransactionAbortedException(timestamp,
                    "Transaction aborted because another transaction wrote or read after (TS < RTS or TS < WTS) ");
            }
            else
            {
                lock (locker)
                {
                    log.AddNewLogEntry(timestamp, uid, newvalue);
                    usedPadInts.Add(uid);
                }
                padint.writeTimestamp = timestamp;
                padint.value = newvalue;
            }
        }

        public int Read(int uid, long timestamp)
        {
            checkFreeze();

            PadInt padint = padInts[uid];
            if (!joinedTx.Contains(timestamp))
            {
                master.TxJoin(myUrl, timestamp);
                lock (locker)
                {
                    localTransactions.Add(timestamp, master.getTransaction(timestamp));
                    joinedTx.Add(timestamp);
                }
            }
            if (padint.writeTimestamp > timestamp)
            {
                master.TxAbort(timestamp);
                throw new TransactionAbortedException(timestamp,
                    "Transaction aborted because a more recent transaction wrote before (WTS > TS)");
            }
            else
            {
                lock (locker)
                {
                    if (padint.writeTimestamp > 0 && padint.writeTimestamp != timestamp)
                    {
                        localTransactions[timestamp].dependencies.Add(padint.writeTimestamp);
                    }
                }
                padint.readTimestamp = Math.Max(timestamp, padint.readTimestamp);
                return padint.value;
            }
        }

        public bool TxPrepare(long timestamp)
        {
            checkFreeze();

            return true;
        }

        public bool TxCommit(long timestamp)
        {
            checkFreeze();

            return joinedTx.Remove(timestamp);
        }

        public bool TxAbort(long timestamp)
        {
            checkFreeze();

            lock (locker)
            {
                log.RemoveAllEntries(timestamp);
                foreach (int uid in usedPadInts)
                {
                    padInts[uid].value = log.getLastValue(uid);
                    padInts[uid].writeTimestamp = log.getLastWriteTimestamp(uid);
                }
                joinedTx.Remove(timestamp);
            }
            return true;
        }

        public bool Status()
        {
            checkFreeze();

            Console.WriteLine("Printing Status");
            Console.WriteLine("---------------");
            Console.WriteLine("Up and Running!");
            return true;
        }

        public void Fail()
        {
            checkFreeze();

            foreach (IChannel channel in ChannelServices.RegisteredChannels)
            {
                ChannelServices.UnregisterChannel(channel);
            }
            Environment.Exit(0);
        }

        public bool Freeze()
        {
            if (this.frozen)
            {
                return false;
            }

            else
            {
                //Monitor.Enter(monitor, ref this.frozen);
                this.frozen = true;
            }
            return true;
        }

        public bool Recover()
        {
            if (!this.frozen)
            {
                return false;
            }
            lock (monitor)
            {
                if (this.frozen)
                {
                    Monitor.PulseAll(monitor);
                    this.frozen = false;
                }
            }
            return true;
        }

        public PadInt CreatePadInt(int uid, PadIntMetadata metadata)
        {
            checkFreeze();

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
            log.AddNewLogEntry(0, uid, 0);
            Console.WriteLine("Created PadInt with uid: " + uid);
            return p;
        }

        public PadInt AccessPadInt(int uid)
        {
            checkFreeze();

            return padInts[uid];
        }

        private void checkFreeze()
        {
            lock (monitor)
            {
                while (this.frozen)
                {
                    Monitor.Wait(monitor);
                }
            }
        }
    }

}
