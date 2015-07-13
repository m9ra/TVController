using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml;

namespace TVControler
{
    class HTTPServer
    {
        bool _end = false;
        int _port;
        object _listenHandShake = new object();

        TcpListener _listener;

        /// <summary>
        /// Create http server which will listen on specified port.
        /// </summary>
        /// <param name="port">Port where http server will be listening</param>
        public HTTPServer(int port)
        {
            _port = port;

            var th = new Thread(_listen);
            th.IsBackground = true;
            lock (_listenHandShake)
            {
                th.Start();
                Monitor.Wait(_listenHandShake);
            }
        }

        /// <summary>
        /// Listen on specified port, while _end is true
        /// </summary>
        private void _listen()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start(50);

            lock (_listenHandShake)
                Monitor.Pulse(_listenHandShake);

            while (!_end)
            {
                TcpClient client;
                //handle incomming clients
                try
                {
                    client = _listener.AcceptTcpClient();
                }
                catch (Exception ex)
                {
                    ConsoleUtils.WriteLn(ex);
                    break;
                }

                var clientTh = new Thread(() => _handleClient(client));
                clientTh.Start();
            }
            _listener.Stop();
        }


        public void Stop()
        {
            _end = true;
            _listener.Stop();
        }

        /// <summary>
        /// Handle specified http client
        /// </summary>
        /// <param name="client">Client to be handled</param>
        private void _handleClient(TcpClient client)
        {
            var stream = client.GetStream();

            //parse incoming http request
            var parser = new HTTPRequestParser();
            while (!parser.IsComplete)
                parser.AddData(stream.Read());

            ConsoleUtils.WriteLn(
                new Wr(ConsoleColor.Yellow, "{1} -> {0}", client.Client.LocalEndPoint, client.Client.RemoteEndPoint),
                new Wr(ConsoleColor.Green, parser.HeaderPart.Replace("{", "{{").Replace("}","}}"))
                );

            proceedRequest(parser, client);
        }

        /// <summary>
        /// Send appropriate response to parsed request
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="client"></param>
        private void proceedRequest(HTTPRequestParser parser, TcpClient client)
        {
            var stream = client.GetStream();
            var response = createResponse(parser);
            if (response == null)
                response = HTTPResponse.FromData(parser, "",500);
            

            stream.Write(response.Headers);
            ConsoleUtils.WriteLn(
             new Wr(ConsoleColor.Yellow, "{0} -> {1}", client.Client.LocalEndPoint, client.Client.RemoteEndPoint),
             new Wr(ConsoleColor.White, response.Headers)
           );

            var method = parser.GetHeader(HTTPRequestParser.Header_Method);

            if (method != "HEAD")
                stream.Write(response.Stream);


            client.Close();
        }

        private HTTPResponse createResponse(HTTPRequestParser parser)
        {
            var soap = parser.GetHeader("SOAPACTION");
            if (soap != null)
                return createSoapResponse(parser, soap);

            var alias = parser.GetHeader(HTTPRequestParser.Header_Uri);
            alias = HTTPProtocol.URLDecode(alias);

            var path = DLNA.Tree.GetFilePath(alias);
            return HTTPResponse.FromFile(parser, path);
        }


        private HTTPResponse createSoapResponse(HTTPRequestParser parser, string soapAction)
        {
            if (!soapAction.Contains("Browse"))
                return null;

            soapAction = soapAction.Replace("\"", "");
            var delim = soapAction.LastIndexOf('#');
            var service = soapAction.Substring(0, delim - 1);
            var action = soapAction.Substring(delim + 1);

            var doc = new XmlDocument();
            doc.LoadXml(parser.Body);

            var id = doc.SelectSingleNode("//ObjectID").InnerText;
            var start = int.Parse(doc.SelectSingleNode("//StartingIndex").InnerText);
            var count = int.Parse(doc.SelectSingleNode("//RequestedCount").InnerText);
            var browseFlag = doc.SelectSingleNode("//BrowseFlag").InnerText;


            DIDLResult didl;
            switch (browseFlag)
            {
                case "BrowseMetadata":
                    //need get object id metadata
                    didl = DLNA.Tree.ToDIDL_Lite_Meta(id);
                    break;
                case "BrowseDirectChildren":
                    //browse all children
                    didl = DLNA.Tree.ToDIDL_Lite(id, start, count);
                    break;
                default:
                    throw new NotSupportedException("BrowseFlag " + browseFlag + " is not supported");
            }

            if (id == "0")
                //root
                id = "";

            
            var browsable = UpnpProtocol.GetBrowseResponse(didl);

            ConsoleUtils.WriteLn(new Wr(ConsoleColor.DarkRed, parser.Body));
             return HTTPResponse.FromData(parser, browsable);            
         //   return HTTPResponse.FromFile(parser, "ContentDirDescription – TV OK.xml");
        }
    }
}
