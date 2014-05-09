using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM
{
    public class ServerMetadata
    {
        private string url;
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        private IDataServer remoteObject;
        public IDataServer RemoteObject
        {
            get { return remoteObject; }
            set { remoteObject = value; }
        }
        private int padintCount;
        public int PadintCount
        {
            get { return padintCount; }
            set { padintCount = value; }
        }
        public ServerMetadata(IDataServer remoteObject, string url)
        {
            this.url = url;
            this.remoteObject = remoteObject;
            this.padintCount = 0;
        }
    }
}
