using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace SystemLauncher
{
    /*
     * Failed attempt to get a way to automate testing and launching multiple data servers
     * But even if you register multiple channels the remote object will be the same :(
     */
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Prepare to launch a complete PADI transactional system");
            Console.WriteLine("Port to run the master");
            int masterPort = Convert.ToInt32(Console.ReadLine());
            PADI_DSTM.MasterServer.launchMasterServer(masterPort);
            Console.WriteLine("How many data servers do you want to run?");
            int nDataServers = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Starting in which port?");
            int firstDataServerPort = Convert.ToInt32(Console.ReadLine());
            int i;
            string masterUrl = "tcp://" + Dns.GetHostName() + ":" + masterPort + "/RemoteMasterServer";

            for (i = 0; i < nDataServers; i++)
            {
                int dataServerPort = firstDataServerPort + i;
                PADI_DSTM.DataServer.launchDataServer(dataServerPort, masterUrl);
                Console.WriteLine("Launched data server in port " + dataServerPort);
            }

            Console.WriteLine("Everything is up and running :)");
            Console.ReadLine();
        }
    }
}
