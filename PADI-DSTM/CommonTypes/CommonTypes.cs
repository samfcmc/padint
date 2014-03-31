using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

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
    }

    public interface IMasterServer : IServer
    {
        bool RegisterDataServer(string url);
        PadIntMetadata CreatePadInt(int uid);
        PadIntMetadata AccessPadInt(int uid);
    }

    public interface IDataServer : IServer
    {
        PadInt CreatePadInt(int uid);
        PadInt AccessPadInt(int uid);
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
    public class PadInt
    {
        private int uid;
        private int value;

        public PadInt(int uid)
        {
            this.uid = uid;
        }

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
