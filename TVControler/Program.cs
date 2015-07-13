using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Net;
using System.Net.Sockets;

using System.Threading;
using System.Windows.Forms;


namespace TVControler
{
    static class SamsungTV
    {

        /*///KODI
        public const string IP = "192.168.1.146";
        public const int ControlPort = 1836;
        /**/

        /**/
        //SAMSUNG
        public const string IP = "192.168.1.53";
        public const int ControlPort = 52235;
        /**/

        public static readonly string Host = string.Format("{0}:{1}", IP, ControlPort);
    }

    class Program
    {
        [STAThread]
        static void Main()
        {
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

            var form = new ControllerForm(null);
            form.Show();

            while (!form.IsClosed)
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }
        }

        static void Mainf(string[] args)
        {
            //SendRequest(UpnpProtocol.SetVolume, 100);

            //return;

            var server = new HTTPServer(42345);
            //       var userver = new UHTTPServer(1900);
            var locIp = NetworkTools.GetLocalIP(SamsungTV.IP);
            var urlBase = "http://" + locIp + ":42345/";

            var pars = new Dictionary<string, string>(){
                {"UUID",UpnpProtocol.UUID},
                {"URLBASE",urlBase}
            };
            XMLTools.CreateFromTemplate("TestDescription.xml", "description.xml", pars);

            //Preparing DLNA tree
            DLNA.SetFolderBinding(@"C:\DLNA_TESTING", urlBase);
            //     DLNA.SetFolderBinding(@"R:\shared", urlBase);
            /*DLNA.Tree.SetLookup("/description.xml", "description.xml");
            DLNA.Tree.SetLookup("/ContentDirectory1.xml", "ContentDirectorySamsung.xml");
            DLNA.Tree.SetLookup("/UPnPServices/ContentDirectory/control/", "ContentDirDescription.xml");
            DLNA.Tree.SetLookup("/UPnPServices/ConnectionManager/description.xml", "description.xml");*/



            //sending play command
            //SendRequest(UpnpProtocol.SetVolume, 2);



            SendRequest(UpnpProtocol.SetAVTransportURI, urlBase + "test.avi");
            SendRequest(UpnpProtocol.Play, 1);

            System.Threading.Thread.Sleep(1000);
            ConsoleUtils.WriteLn(new Wr(ConsoleColor.White, "Enabling upnp services"));
            foreach (var service in UpnpProtocol.Service_Descriptors)
            {
                var serviceName = string.Format(service, UpnpProtocol.UUID);
                //        userver.AddUpnpService(serviceName, urlBase + "description.xml");
            }

            while (Console.ReadKey().Key != ConsoleKey.Escape) ;

            //      userver.Stop();
            server.Stop();/**/

            System.Threading.Thread.Sleep(500);

        }

        static void SendRequest(UpnpRequest req, params object[] pars)
        {
            var http = req.GetHttp(SamsungTV.Host, pars);
            SendHTTP(http);
        }

        private static void SendHTTP(string http, params object[] pars)
        {
            http = string.Format(http, pars);
            var tv = new TcpClient();
            tv.Connect(SamsungTV.IP, SamsungTV.ControlPort);

            var stream = tv.GetStream();
            stream.Write(http);
            var parser = new HTTPRequestParser();

            while (!parser.IsComplete)
            {
                var data = stream.Read();
                parser.AddData(data);
            }
            tv.Close();
        }
    }
}
