using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace PADI_DSTM
{
    public interface IServer
    {
        bool TxCommit(long timestamp);
        bool TxAbort(long timestamp);
        bool Status();
        void Fail();
        bool Freeze();
        bool Recover();
    }

    public interface IMasterServer : IServer
    {
        string NotifyFault(string notifier, string faultyServer);
        Dictionary<string, IDataServer> getDataServers();
        PadiTransaction getTransaction(long timestamp);
        long TxBegin();
        bool TxJoin(string url, long timestamp);
        bool RegisterDataServer(string url);
        PadIntMetadata CreatePadInt(int uid);
        PadIntMetadata AccessPadInt(int uid);
    }

    public interface IDataServer : IServer
    {
        void StorePadInt(int uid, PadInt p);
        bool Echo();
        string GetNextServer();
        void SetNextServer(string next);
        List<long> GetTxDependencies(long timestamp);
        bool TxPrepare(long timestamp);
        int Read(int uid, long timestamp);
        void Write(int uid, long timestamp, int newvalue);
        PadInt CreatePadInt(int uid, PadIntMetadata metadata);
        PadInt AccessPadInt(int uid);
    }

    [Serializable]
    public class PadIntMetadata
    {
        public int uid;
        public List<string> servers;

        public PadIntMetadata()
        {

        }
    }

    [Serializable]
    public enum STATE {
        RUNNING,
        COMMITTED,
        ABORTED
    }

    [Serializable]
    public class PadiTransaction
    {
        public long timestamp;
        public List<string> servers;
        public STATE state;
        public List<long> dependencies = new List<long>();

        public PadiTransaction(long timestamp)
        {
            this.timestamp = timestamp;
            servers = new List<string>();
        }
    }

    [Serializable]
    public class PadInt
    {
        private int uid;
        public int value;
        public long readTimestamp = 0;
        public long writeTimestamp = 0;
        public List<string> servers;

        public PadInt(int uid)
        {
            this.uid = uid;
            servers = new List<string>();
        }

        public int Read()
        {
            if (PadiDstm.currentTimestamp < 0)
            {
                throw new Exception("Error: Cannot read outside a transaction.");
            }

            int val = 0;
            foreach (string s in servers)
            {
                IDataServer server = (IDataServer)Activator.GetObject(
                    typeof(IDataServer),
                    s);
                val = TryRead(server);
                break;
            }
            return val;
        }

        private int TryRead(IDataServer server)
        {
            int tries = 10;
            while (tries > 0)
            {
                try
                {
                    return server.Read(this.uid, PadiDstm.currentTimestamp);
                }
                catch (SocketException e)
                {
                    PadInt refreshed = PadiDstm.AccessPadInt(uid);
                    servers = refreshed.servers;
                    tries--;
                }
            }
            throw new Exception("Timeout Error: Could not reach the servers where the PadInt was stored.");
        }

        public void Write(int val)
        {
            if (PadiDstm.currentTimestamp < 0)
            {
                throw new Exception("Error: Cannot write outside a transaction.");
            }
            foreach (string s in servers)
            {
                IDataServer server = (IDataServer)Activator.GetObject(
                    typeof(IDataServer),
                    s);
                TryWrite(server, val);
            }
        }

        private void TryWrite(IDataServer server, int val)
        {
            int tries = 10;
            while(tries > 0) {
                try
                {
                    server.Write(this.uid, PadiDstm.currentTimestamp, val);
                    return;
                }
                catch (SocketException e)
                {
                    PadInt refreshed = PadiDstm.AccessPadInt(uid);
                    servers = refreshed.servers;
                    tries--;
                }
            }
        }
    }

    public class PadiDstm
    {
        public static IMasterServer masterServer;
        public static string masterPort = "8086";
        public static string masterHostname = "localhost";
        public static long currentTimestamp = -1;

        public static bool Init()
        {
            /*Console.WriteLine("What is the master port?");
            masterPort = Console.ReadLine();

            Console.WriteLine("What is the master hostname?");
            masterHostname = Console.ReadLine();*/

            masterServer = (IMasterServer)Activator.GetObject(
                typeof(IMasterServer),
                "tcp://" + masterHostname + ":" + masterPort + "/RemoteMasterServer");

            return true;
        }

        public static bool TxBegin()
        {
            currentTimestamp = masterServer.TxBegin();
            return true;
        }

        public static bool TxCommit()
        {
            return masterServer.TxCommit(currentTimestamp);
        }

        public static bool TxAbort()
        {
            return masterServer.TxAbort(currentTimestamp);
        }

        public static bool Status()
        {
            Console.WriteLine("Printing Status");
            Console.WriteLine("---------------");
            if (masterServer.Status())
            {
                Console.WriteLine("MasterServer is alive.");
            }

            foreach (KeyValuePair<string, IDataServer> server in masterServer.getDataServers())
            {
                if (server.Value.Status())
                {
                    Console.WriteLine("DataServer " + server.Key);
                }
            }
            return true;
        }

        public static bool Fail(string url)
        {
            IDataServer dataServer = (IDataServer)Activator.GetObject(
                typeof(IDataServer),
                url);

            try
            {
                dataServer.Fail();
            }
            catch (Exception e)
            {
                //Something went wrong trying to fail the server
                return false;
            }

            return true;
        }

        public static bool Freeze(string url)
        {
            IDataServer server = (IDataServer)Activator.GetObject(
                    typeof(IDataServer),
                    url);

            return server.Freeze();
        }

        public static bool Recover(string url)
        {
            IDataServer server = (IDataServer)Activator.GetObject(
                    typeof(IDataServer),
                    url);
            return server.Recover();
        }

        public static PadInt CreatePadInt(int uid)
        {
            PadIntMetadata metadata = masterServer.CreatePadInt(uid);
            if (metadata == null)
            {
                return null;
            }

            PadInt p = null;
            foreach (string s in metadata.servers)
            {
                IDataServer server = (IDataServer)Activator.GetObject(
                    typeof(IDataServer),
                    s);
                p = server.CreatePadInt(uid, metadata);
            }
            return p;
        }

        public static PadInt AccessPadInt(int uid)
        {
            PadIntMetadata metadata = masterServer.AccessPadInt(uid);

            foreach (string s in metadata.servers)
            {
                IDataServer server = (IDataServer)Activator.GetObject(
                    typeof(IDataServer),
                    s);
                return server.AccessPadInt(uid);
            }
            return null;
        }
    }
}
