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
        long TxBegin();
        bool TxJoin(string url, long timestamp);
        bool RegisterDataServer(string url);
        PadIntMetadata CreatePadInt(int uid);
        PadIntMetadata AccessPadInt(int uid);
    }

    public interface IDataServer : IServer
    {
        bool TxBegin(int uid, long timestamp);
        bool TxPrepare(long timestamp);
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
        public long currentTimestamp;
        private int value;
        public List<string> servers;

        public PadInt(int uid)
        {
            this.uid = uid;
            this.currentTimestamp = -1;
            servers = new List<string>();
        }

        public int Read()
        {
            foreach(string s in servers) 
            {
                IDataServer server = (IDataServer)Activator.GetObject(
                    typeof(IDataServer),
                    s);
                //server.TxBegin();
            }

            return value;
        }

        public void Write(int val)
        {
            value = val;
        }
    }

}
