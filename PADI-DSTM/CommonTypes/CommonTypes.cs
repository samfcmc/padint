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
}
