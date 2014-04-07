using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PADI_DSTM
{
    class Client
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CLIENT APPLICATION v1.0");

            bool res;

            PadiDstm.Init();

            res = PadiDstm.TxBegin();
            PadInt pi_a = PadiDstm.CreatePadInt(0);
            PadInt pi_b = PadiDstm.CreatePadInt(1);
            res = PadiDstm.TxCommit();

            res = PadiDstm.TxBegin();
            pi_a = PadiDstm.AccessPadInt(0);
            pi_b = PadiDstm.AccessPadInt(1);
            res = PadiDstm.Freeze("tcp://localhost:1001/RemoteDataServer");
            pi_a.Write(36);
            pi_b.Write(37);
            Console.WriteLine("a = " + pi_a.Read());
            Console.WriteLine("b = " + pi_b.Read());
            PadiDstm.Status();
            // The following 3 lines assume we have 2 servers: one at port 1001 and another at port 1002
            
            res = PadiDstm.Recover("tcp://localhost:1001/RemoteDataServer");
            res = PadiDstm.Fail("tcp://localhost:1002/RemoteDataServer");
            res = PadiDstm.TxCommit();
        }
    }
}
