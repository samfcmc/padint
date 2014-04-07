using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM
{
    public class PadiDstm
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
}
