using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM
{
    public class PadiDstm : PadiLib
    {
        static IMasterServer masterServer;
        static string masterPort;
        static string masterHostname;

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
            return false;
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
            return false;
        }

        public static bool Freeze(string url)
        {
            return false;
        }

        public static bool Recover(string url)
        {
            return false;
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
                p = server.CreatePadInt(uid);
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

    class Client
    {

        static void Main(string[] args)
        {
            Console.WriteLine("CLIENT APPLICATION v1.0");

            bool res;

            PadiDstm.Init();

            res = PadiDstm.TxBegin();
            PadInt pi_a = PadiDstm.CreatePadInt(0);
            PadInt pi_b = PadiDstm.CreatePadInt(1);
            res = PadiDstm.TxCommit();

            res = PadiDstm.TxBegin();
            pi_a = PadiDstm.AccessPadInt(0);
            pi_b = PadiDstm.AccessPadInt(1);
            pi_a.Write(36);
            pi_b.Write(37);
            Console.WriteLine("a = " + pi_a.Read());
            Console.WriteLine("b = " + pi_b.Read());
            PadiDstm.Status();
            // The following 3 lines assume we have 2 servers: one at port 1001 and another at port 1002
            res = PadiDstm.Freeze("tcp://localhost:1001/Server");
            res = PadiDstm.Recover("tcp://localhost:1001/Server");
            res = PadiDstm.Fail("tcp://localhost:1002/Server");
            res = PadiDstm.TxCommit();
        }
    }
}
