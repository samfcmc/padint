using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        Dictionary<string, IDataServer> getDataServers();
        long TxBegin();
        bool TxJoin(string url, long timestamp);
        bool RegisterDataServer(string url);
        PadIntMetadata CreatePadInt(int uid);
        PadIntMetadata AccessPadInt(int uid);
    }

    public interface IDataServer : IServer
    {
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

    public class PadiTransaction
    {
        public long timestamp;
        public List<string> servers;

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
        public long readTimestamp;
        public long writeTimestamp;
        public List<string> servers;

        public PadInt(int uid)
        {
            this.uid = uid;
            servers = new List<string>();
        }

        public int Read()
        {
            foreach (string s in servers)
            {
                IDataServer server = (IDataServer)Activator.GetObject(
                    typeof(IDataServer),
                    s);
                try
                {
                    return server.Read(this.uid, PadiDstm.currentTimestamp);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
            throw new Exception("Error: Could not contact the data servers.");
        }

        public void Write(int val)
        {
            foreach (string s in servers)
            {
                IDataServer server = (IDataServer)Activator.GetObject(
                    typeof(IDataServer),
                    s);
                server.Write(this.uid, PadiDstm.currentTimestamp, val);
            }
        }
    }

    public class PadiDstm
    {
        public static IMasterServer masterServer;
        public static string masterPort;
        public static string masterHostname;
        public static long currentTimestamp;

        public static bool Init()
        {
            Console.WriteLine("What is the master port?");
            masterPort = Console.ReadLine();

            Console.WriteLine("What is the master hostname?");
            masterHostname = Console.ReadLine();

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
            return false;
        }

        public static bool TxAbort()
        {
            return false;
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
