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
        List<long> joinedTx = new List<long>();
        public static string myUrl;
        public static IMasterServer master;

        public void Write(int uid, long timestamp, int newvalue)
        {
            if (!joinedTx.Contains(timestamp))
            {
                master.TxJoin(myUrl, timestamp);
                joinedTx.Add(timestamp);
            }
            if (timestamp < padInts[uid].readTimestamp)
            {
                throw new Exception("TransactionAbortedException");
            }
            else if (timestamp > padInts[uid].writeTimestamp)
            {
                padInts[uid].writeTimestamp = timestamp;
                padInts[uid].value = newvalue;
            }
        }

        public int Read(int uid, long timestamp)
        {
            if (!joinedTx.Contains(timestamp))
            {
                master.TxJoin(myUrl, timestamp);
                joinedTx.Add(timestamp);
            }
            if (padInts[uid].writeTimestamp > timestamp)
            {
                throw new Exception("TransactionAbortedException");
            }
            else if (padInts[uid].readTimestamp < timestamp)
            {
                padInts[uid].readTimestamp = timestamp;
            }
            return padInts[uid].value;
        }

        public bool TxPrepare(long timestamp)
        {
            return true;
        }

        public bool TxCommit(long timestamp)
        {
            return false;
        }

        public bool TxAbort(long timestamp)
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
