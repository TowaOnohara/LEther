using System;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Net.NetworkInformation;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace test
{
    public partial class Program
    {
        bool DebugLedValue = false;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            InitializeNetwork();

            new Thread(RunWebServer).Start();
            GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
            timer.Tick += timer_Tick; 
            timer.Start();

            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");
        }

        void timer_Tick(GT.Timer timer)
        {
            Mainboard.SetDebugLED(DebugLedValue = !DebugLedValue);
        }


        private void DoResponse(HttpListenerContext context)
        {
            var str = "<html><head><title>Hello I'm fine.</title><body><h1>Cerberus Infomation</h1>" +
                "<p>Time    = " + DateTime.Now.ToString() + "</p>" +
                "</body>";
            var outBuffer = Encoding.UTF8.GetBytes(str);

            var response = context.Response;
            response.OutputStream.Write(outBuffer, 0, outBuffer.Length);
        }

        private void RunWebServer()
        {
            var listener = new HttpListener("http");
            listener.Start();

            while (true)
            {
                HttpListenerResponse response = null;
                try
                {
                    var context = listener.GetContext();
                    response = context.Response;
                    var request = context.Request;
                    if (request.HttpMethod.ToUpper() == "GET")
                    {
                        led7r.Animate(100, true, true, false);
                        DoResponse(context);
                    }
                }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                    }
                    led7r.Animate(100, true, true, false);
                    led7r.TurnLightOff(6);
                }
            }
        }

        private void InitializeNetwork()
        {
            Debug.Print("Initializing network…");
            var netif = NetworkInterface.GetAllNetworkInterfaces()[0];

            Debug.Print("Network ready.");
            Debug.Print("  IP Address: " + netif.IPAddress);
            Debug.Print("  Subnet Mask: " + netif.SubnetMask);

            Thread.Sleep(1000);
        }
    }
}