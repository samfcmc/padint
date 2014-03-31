using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM
{
    public class PadiDstm : PadiLib
    {
        IMasterServer masterServer;

        public bool Init()
        {
            Console.WriteLine("What is the master port?");
            string master_port = Console.ReadLine();

            Console.WriteLine("What is the master hostname?");
            string master_hostname = Console.ReadLine();

            masterServer = (IMasterServer)Activator.GetObject(
                typeof(IMasterServer),
                "tcp://" + master_hostname + ":" + master_port + "/RemoteMasterServer");

            return masterServer.Status();
        }

        public bool TxBegin()
        {
            return false;
        }

        public bool TxCommit()
        {
            return false;
        }

        public bool TxAbort()
        {
            return false;
        }

        public bool Status()
        {
            Console.WriteLine("Printing Status");
            Console.WriteLine("---------------");
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
            PadIntMetadata metadata = masterServer.CreatePadInt(uid);
            
            foreach (string s in metadata.servers)
            {
                IDataServer server = (IDataServer)Activator.GetObject(
                    typeof(IDataServer),
                    s);
                server.CreatePadInt(uid);
            }
            return new PadInt(uid);
        }

        public PadInt AccessPadInt(int uid) 
        {
            //TODO: fix access
            return new PadInt(uid);
        }
    }

    class Client
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CLIENT APPLICATION v1.0");

            PadiDstm dstm = new PadiDstm();

            dstm.Init();

            PadInt p = dstm.CreatePadInt(0);
            p.Write(30);
            
            Console.WriteLine(p.Read());

            Console.ReadLine();
        }
    }
}
