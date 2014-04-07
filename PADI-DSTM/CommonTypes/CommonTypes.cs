using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PADI_DSTM
{
    public interface IServer
    {
        bool TxCommit(long timestamp);
        bool TxAbort(long timestamp);
        bool Status();
        bool Fail(string url);
        bool Freeze(string url);
        bool Recover(string url);
    }

    public interface IMasterServer : IServer
    {
        Dictionary<string, IDataServer> getDataServers();
        bool TxBegin();
        bool RegisterDataServer(string url);
        PadIntMetadata CreatePadInt(int uid);
        PadIntMetadata AccessPadInt(int uid);
    }

    public interface IDataServer : IServer
    {
        bool TxJoin(long timestamp);
        bool TxBegin(long timestamp);
        bool TxPrepare(long timestamp);
        PadInt CreatePadInt(int uid);
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
        public long currentTimestamp;
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
