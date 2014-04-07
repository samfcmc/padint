﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

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

            IMasterServer master = (IMasterServer)Activator.GetObject(
                typeof(IMasterServer),
                "tcp://" + master_hostname + ":" + master_port + "/RemoteMasterServer");

            bool reg = master.RegisterDataServer("tcp://" + Dns.GetHostName() + ":" + port + "/RemoteDataServer");
            
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

        public bool TxBegin(long timestamp)
        {
            return false;
        }

        public bool TxJoin(long timestamp)
        {
            return false;
        }

        public bool TxPrepare(long timestamp)
        {
            return false;
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
            if (padInts.ContainsKey(uid))
            {
                return null;
            }
            PadInt p = new PadInt(uid);
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
