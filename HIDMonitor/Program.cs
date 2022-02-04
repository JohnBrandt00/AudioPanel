using Ex.HidInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIDMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            string deviceName = "GMMK Pro";
            string vendorID = "0x320F";
            string productID = "0x5044";
            string usagePage = "0xFF60";
            string usageID = "0x61";

            // Requested action and its context for HidDevice
            //HostInterface.HidAction action = HostInterface.HidAction.ChangeLayer; // Action to execute on HidDevice
            //int context = 1; // Context for desired HidAction

            //// Create new HostInterface instance and pass it target HidDevice's information
            //// It is strongly recommended that all the below information be provided (your mileage may vary)
            HostInterface hostInterface = new HostInterface(deviceName, vendorID, productID, usagePage, usageID);

            //// Connect with target HidDevice and engage automatic 'listening' for HidDevice data messages
            hostInterface.Connect(true);

            //Console.WriteLine("Press any key to send message to HidDevice"); // Output info to event log (debug)
            //Console.ReadLine();

            var client = new NamedPipeClientStream("HIDPipe");
            client.Connect();
            StreamReader reader = new StreamReader(client);
            StreamWriter writer = new StreamWriter(client);

            while (true)
            {
                string input = Console.ReadLine();
                if (String.IsNullOrEmpty(input)) break;
                writer.WriteLine(input);
                writer.Flush();
                //Console.WriteLine(reader.ReadLine());
            }


        }
    }
}
