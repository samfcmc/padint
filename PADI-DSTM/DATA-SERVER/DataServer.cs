using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM
{
    class DataServer
    {
        static void Main(string[] args)
        {
        }
    }

    public class RemoteDataServer : MarshalByRefObject, IServer
    {
        public bool Init()
        {
            return true;
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
            return new PadInt();
        }

        public PadInt AccessPadInt(int uid)
        {
            return null;
        }
    }
}
