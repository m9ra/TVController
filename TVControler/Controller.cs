using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Threading;

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

        public bool IsConnected { get { return _isConnected; } }

        public PlayInfo CurrentInfo { get; private set; }

        private object _L_operations = new object();

        private volatile bool _isConnected;

        private volatile bool _stop = false;

        private readonly List<Action> _operations = new List<Action>();

        private readonly ControlerServer _server;

        private readonly Thread _thread;

        public Controller(string hostIp, int controlPort)
        {
            _server = new ControlerServer(LocalPort);
            RemoteIP = hostIp;
            RemotePort = controlPort;

            RemoteUri = RemoteIP + ":" + controlPort;

            _thread = new Thread(_run);
            _thread.IsBackground = true;
            _thread.Start();
        }

        internal static string ToTimeStr(int secondsFromStart)
        {
            var timeSpan = new TimeSpan(0, 0, secondsFromStart);
            var time = string.Format("{0:0}:{1:00}:{2:00}", (int)timeSpan.TotalHours, (int)timeSpan.Minutes, (int)timeSpan.Seconds);
            return time;
        }

        public void PlayFile(string file)
        {
            signalAction(() => _PlayFile(file));
        }

        internal void IncrementVolume(int delta)
        {
            signalAction(() => _IncrementVolume(delta));
        }

        internal void SeekTo(int seconds)
        {
            signalAction(() => _SeekTo(seconds));
        }

        internal void Stop()
        {
            signalAction(() => _Stop());
        }

        internal void Pause()
        {
            signalAction(() => _Pause());
        }

        internal void Play()
        {
            signalAction(() => _Play());
        }

        #region Asynchronous processing

        private void _run()
        {
            while (!_stop)
            {
                CurrentInfo = getInfo();

                IEnumerable<Action> pendingOperations;
                lock (_L_operations)
                {
                    if (_operations.Count == 0)
                        //wait for next operations
                        Monitor.Wait(_L_operations, 1000);

                    pendingOperations = _operations.ToArray();
                    _operations.Clear();
                }

                foreach (var pendingOperation in pendingOperations)
                {
                    pendingOperation();
                }

                Thread.Sleep(10);
            }
        }

        private void signalAction(Action action)
        {
            lock (_L_operations)
            {
                _operations.Add(action);
                Monitor.Pulse(_L_operations);
            }
        }

        #endregion

        #region Operation implementation

        private void _IncrementVolume(int delta)
        {
            var volumeResponse = sendRequest(UpnpProtocol.GetVolume);
            if (volumeResponse == null)
                //we are not connected
                return;

            var volumeString = parseOutTag(volumeResponse.Body, "CurrentVolume");
            int volume;
            int.TryParse(volumeString, out volume);
            var newVolume = volume + delta;

            sendRequest(UpnpProtocol.SetVolume, newVolume);
        }

        private void _SeekTo(int seconds)
        {
            sendRequest(UpnpProtocol.SeekTo, ToTimeStr(seconds));
        }

        private void _Stop()
        {
            sendRequest(UpnpProtocol.Stop);
        }

        private void _Pause()
        {
            sendRequest(UpnpProtocol.Pause);
        }

        private void _Play()
        {
            sendRequest(UpnpProtocol.Play, 1);
        }

        private void _PlayFile(string file)
        {
            var locIp = NetworkTools.GetLocalIP(RemoteIP);
            var urlBase = "http://" + locIp + ":" + LocalPort + "/";

            var encodedFilePath = "b64_" + Convert.ToBase64String(Encoding.UTF8.GetBytes(file)).Replace('=', '_');

            var transportParser = sendRequest(UpnpProtocol.SetAVTransportURI, urlBase + encodedFilePath);
            if (transportParser == null)
                return;

            _Play();  
        }

        #endregion

        private PlayInfo getInfo()
        {
            var positionInfo = sendRequest(UpnpProtocol.GetPosition);
            var transportInfo = sendRequest(UpnpProtocol.GetTransportInfo);
            if (positionInfo == null || transportInfo == null)
                return null;

            Console.WriteLine(transportInfo.Body);
            var positionInfoHTML = positionInfo.Body;
            var transportInfoHTML = transportInfo.Body;
            var duration = parseOutTag(positionInfoHTML, "TrackDuration");
            var actualTime = parseOutTag(positionInfoHTML, "RelTime");
            var uri = parseOutTag(positionInfoHTML, "TrackURI");
            var currentTransportState = parseOutTag(transportInfoHTML, "CurrentTransportState");


            var durationSeconds = (int)TimeSpan.Parse(duration).TotalSeconds;
            var actualTimeSeconds = (int)TimeSpan.Parse(actualTime).TotalSeconds;
            return new PlayInfo(uri, actualTimeSeconds, durationSeconds, currentTransportState);
        }

        private string parseOutTag(string source, string tag)
        {
            var prefix = "<" + tag + ">";
            var startIndex = source.IndexOf(prefix) + prefix.Length;
            var endIndex = source.IndexOf("</", startIndex);

            var parsedString = source.Substring(startIndex, endIndex - startIndex);
            return parsedString;
        }

        private HTTPRequestParser sendRequest(UpnpRequest req, params object[] pars)
        {
            var http = req.GetHttp(RemoteUri, pars);
            HTTPRequestParser parser = null;
            try
            {
                parser = sendHTTP(http);
            }
            catch (SocketException)
            {
                //problem with connection
            }

            if (parser == null)
            {
                //failed to connect
                _isConnected = false;
                CurrentInfo = null;
            }

            return parser;
        }

        private HTTPRequestParser sendHTTP(string http, params object[] pars)
        {
            http = string.Format(http, pars);
            var tv = new TcpClient();
            var result = tv.BeginConnect(RemoteIP, RemotePort, null, null);

            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));
            if (!success)
            {
                return null;
            }

            // we have connected
            tv.EndConnect(result);

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
