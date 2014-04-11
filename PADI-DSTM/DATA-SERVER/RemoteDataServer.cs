﻿using PADI_DSTM.Exceptions;
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
        public static string myUrl;
        public static IMasterServer master;

        bool frozen = false;
        object test = new object();

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
                throw new TransactionAbortedException(timestamp,
                    "Transaction aborted because another transaction wrote or read after (TS < RTS or TS < WTS) ");
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
                throw new TransactionAbortedException(timestamp,
                    "Transaction aborted because some transaction wrote before (WTS > TS)");
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
            Monitor.Enter(test, ref this.frozen);
            return true;
        }

        public bool Recover()
        {
            Monitor.PulseAll(this.frozen);
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
            lock(test) {
                while (this.frozen)
                {
                    Monitor.Wait(test);
                    this.frozen = false;
                }
            }           
        }

    }
}
