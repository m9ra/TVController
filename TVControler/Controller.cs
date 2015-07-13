using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using System.Net;
using System.Net.Sockets;

namespace TVControler
{

    class Controller
    {
        public readonly string RemoteIP;

        public readonly string RemoteUri;

        public readonly int RemotePort;

        public readonly int LocalPort = 42345;

        private readonly ControlerServer _server;

        public Controller(string hostIp, int controlPort)
        {
            _server = new ControlerServer(LocalPort);

            RemoteIP = hostIp;
            RemotePort = controlPort;

            RemoteUri = RemoteIP + ":" + controlPort;
        }

        public void PlayFile(string file)
        {

            var locIp = NetworkTools.GetLocalIP(RemoteIP);
            var urlBase = "http://" + locIp + ":" + LocalPort + "/";

            var encodedFilePath = Convert.ToBase64String(Encoding.UTF8.GetBytes(file)).Replace('=', '_');

            var transportParser = sendRequest(UpnpProtocol.SetAVTransportURI, urlBase + encodedFilePath);
            if (transportParser != null)
                ConsoleUtils.WriteLn(
                    new Wr(ConsoleColor.Green, "SetAVTransportURI '{0}'", urlBase + encodedFilePath),
                    new Wr(ConsoleColor.Yellow, "Header '{0}'", transportParser.HeaderPart),
                    new Wr(ConsoleColor.Yellow, "Body '{0}'", transportParser.Body)
                    );


            var playParser = sendRequest(UpnpProtocol.Play, 1);
            if (playParser != null)
                ConsoleUtils.WriteLn(
                    new Wr(ConsoleColor.Green, "Play'"),
                    new Wr(ConsoleColor.Yellow, "Header '{0}'", playParser.HeaderPart),
                    new Wr(ConsoleColor.Yellow, "Body '{0}'", playParser.Body)
                    );

        }

        internal void IncrementVolume(int delta)
        {
            var volumeResponse = sendRequest(UpnpProtocol.GetVolume);

            var volumePrefix = "<CurrentVolume>";
            var volumeStartIndex = volumeResponse.Body.IndexOf(volumePrefix) + volumePrefix.Length;
            var volumeEndIndex = volumeResponse.Body.IndexOf("<", volumeStartIndex);

            var volumeString = volumeResponse.Body.Substring(volumeStartIndex, volumeEndIndex - volumeStartIndex);
            int volume;
            int.TryParse(volumeString, out volume);
            var newVolume = volume + delta;

            sendRequest(UpnpProtocol.SetVolume, newVolume);
        }

        internal void Stop()
        {
            sendRequest(UpnpProtocol.Stop);
        }

        private HTTPRequestParser sendRequest(UpnpRequest req, params object[] pars)
        {
            try
            {
                var http = req.GetHttp(RemoteUri, pars);
                return sendHTTP(http);
            }
            catch (SocketException ex)
            {
                return null;
            }
        }

        private HTTPRequestParser sendHTTP(string http, params object[] pars)
        {
            http = string.Format(http, pars);
            var tv = new TcpClient();
            tv.Connect(RemoteIP, RemotePort);

            var stream = tv.GetStream();
            stream.Write(http);
            var parser = new HTTPRequestParser();
            while (!parser.IsComplete)
            {
                var data = stream.Read();
                parser.AddData(data);
            }
            tv.Close();

            return parser;
        }
    }
}
