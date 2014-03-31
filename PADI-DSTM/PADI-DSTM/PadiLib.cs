using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM
{
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
