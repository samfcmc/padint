using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM
{
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

    public interface PadiLib
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
}
