using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TVControler
{
    class UHTTPServer
    {
        int _port;
        bool _end = false;
        object _listenHandShake = new object();

        /// <summary>
        /// Services that are handled as active
        /// </summary>
        Dictionary<string, string> _upnpServices = new Dictionary<string, string>();

        UdpClient _listener;
        UdpClient _sender;

        public UHTTPServer(int port)
        {
            _listener = new UdpClient();
            _sender = new UdpClient();
            _listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            _sender.EnableBroadcast = true;
            _sender.MulticastLoopback = true;

            var localpt = new IPEndPoint(IPAddress.Any, port);
            _listener.Client.Bind(localpt);
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
            //       _listener.Client.Listen(500);
            lock (_listenHandShake)
                Monitor.Pulse(_listenHandShake);

            IPEndPoint clientEndPoint;

            var processedClients = new Dictionary<string, HTTPRequestParser>();
            _listener.EnableBroadcast = true;
            _listener.MulticastLoopback = true;

            var address = IPAddress.Parse("239.255.255.250");
            _listener.JoinMulticastGroup(address);

            while (!_end)
            {
                //handle incomming reqs
                string data;
                try
                {
                    clientEndPoint = new IPEndPoint(IPAddress.Any, _port);
                    var buffer = _listener.Receive(ref clientEndPoint);
                    data = Encoding.ASCII.GetString(buffer);
                }
                catch (Exception ex)
                {
                    ConsoleUtils.WriteLn(ex);
                    break;
                }


                var clientKey = clientEndPoint.ToString();
                HTTPRequestParser parser;
                if (!processedClients.TryGetValue(clientKey, out parser))
                    processedClients.Add(clientKey, parser = new HTTPRequestParser());

                parser.AddData(data);
                if (parser.IsComplete)
                {
                    processedClients.Remove(clientKey);
                    onReqCompleted(parser, clientEndPoint);
                }

            }
            _listener.Close();
        }


        public void Stop()
        {
            var toRemove = _upnpServices.Keys.ToArray();

            foreach (var service in toRemove)
            {
                DisableUpnpService(service);
            }

            _end = true;
            Thread.Sleep(500);
            _listener.Close();
        }


        private void onReqCompleted(HTTPRequestParser request, IPEndPoint endpoint)
        {
            ConsoleUtils.WriteLn(
                new Wr(ConsoleColor.Yellow, "UHTTP recieved {0}", endpoint),
                new Wr(ConsoleColor.Cyan, request.HeaderPart)
                );

            if (request.GetHeader(HTTPRequestParser.Header_Method).Contains("SEARCH"))
            {
                var search = request.GetHeader("ST");
                if (search == "ssdp:all")
                {
                    throw new NotImplementedException("response with all services");
                }

                string location;
                if (_upnpServices.TryGetValue(search, out location))
                {
                    var th = new Thread(() =>
                    {
                        //Positive replay to search
                        
                        try
                        {
                            SendTo(endpoint, HTTPProtocol.Headers_SEARCH_ok, location, search, UpnpProtocol.GetUSN(UpnpProtocol.UUID, search));
                        }
                        catch (Exception ex)
                        {
                            ConsoleUtils.WriteLn(ex);
                        }
                    });

                    th.IsBackground = true;
                    th.Start();
                }
            }
        }

        public void Send(string uhttp, params object[] formatArgs)
        {
            SendTo(new IPEndPoint(IPAddress.Parse("239.255.255.250"), _port), uhttp, formatArgs);
        }

        public void SendTo(IPEndPoint remotePoint, string uhttp, params object[] formatArgs)
        {
            uhttp = string.Format(uhttp, formatArgs);
            uhttp = uhttp.Replace("\r", "");
            uhttp = uhttp.Replace("\n", "\r\n");

            ConsoleUtils.WriteLn(
              new Wr(ConsoleColor.Yellow, "UHTTP sended {0}", remotePoint),
              new Wr(ConsoleColor.Gray, uhttp)
              );

            var bytes = Encoding.ASCII.GetBytes(uhttp);

            var sended = _sender.Send(bytes, bytes.Length, remotePoint);
            while (_sender.Available > 0)
            {
                var endP = new IPEndPoint(IPAddress.Any, 0);
                var data = _sender.Receive(ref endP);
                var dataStr = Encoding.UTF8.GetString(data);
                ConsoleUtils.WriteLn(
                   new Wr(ConsoleColor.Yellow, "UHTTP response recieved {0}", endP),
                   new Wr(ConsoleColor.Blue, dataStr)
                   );
            }
            if (sended != bytes.Length)
                throw new NotSupportedException("fragmenting is not supported");
        }

        /// <summary>
        /// Add upnp service into notification/denotification/searchable service system
        /// </summary>
        /// <param name="service"></param>
        public void AddUpnpService(string service, string location)
        {
            _upnpServices.Add(service, location);

            var usn = UpnpProtocol.GetUSN(UpnpProtocol.UUID, service);
            Send(HTTPProtocol.Headers_NOTIFY_alive, service, location, UpnpProtocol.UUID, usn);
        }

        /// <summary>
        /// Remove upnp service and send byebye notification
        /// </summary>
        /// <param name="service"></param>
        public void DisableUpnpService(string service)
        {
            var usn = UpnpProtocol.GetUSN(UpnpProtocol.UUID, service);

            Send(HTTPProtocol.Headers_NOTIFY_byebye, service, UpnpProtocol.UUID, usn);
            _upnpServices.Remove(service);
        }
    }
}
