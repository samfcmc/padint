using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM
{
    public interface IServer
    {
        bool Init();
        bool TxBegin();
        bool TxCommit();
        bool TxAbort();
        bool Status();
        bool Fail(string url);
        bool Freeze(string url);
        bool Recover(string url);

        PadInt CreatePadInt(int uid);
        PadInt AccessPadInt(int uid);
    }

    public interface IMasterServer : IServer
    {
        bool RegisterDataServer(string url);
    }

    public class Transaction
    {
        private uint id;

        public Transaction(uint id)
        {
            this.id = id;
        }

        public uint getID()
        {
            return id;
        }
    }

    public class Server
    {
        public string url;
    }

    public class PadIntMetadata
    {
        Dictionary<int, List<Server>> PadIntLocations;

    }

    public class PadInt
    {
        private int value;

        public int Read()
        {
            return value;
        }

        public void Write(int val)
        {
            value = val;
        }
    }

}
