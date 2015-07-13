using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace TVControler
{
    static class NetworkTools
    {
        public static void Write(this NetworkStream stream, string data)
        {
            var buff = Encoding.UTF8.GetBytes(data);
            stream.Write(buff, 0, buff.Length);
        }

        public static void Write(this NetworkStream stream, ResponseStream data)
        {
            byte[] buffer = new byte[10000];
            try
            {
                while (!data.End && stream.CanWrite)
                {
                    var len = data.GetChunk(buffer);
                    stream.Write(buffer, 0, len);
                }
                stream.Flush();
            }
            catch (Exception ex)
            {
                ConsoleUtils.WriteLn(ex);
            }
        }

        public static string Read(this NetworkStream stream)
        {
            try
            {
                var buff = new byte[10000];
                var cn = stream.Read(buff, 0, buff.Length - 1);

                var res = Encoding.UTF8.GetString(buff, 0, cn);

                return res;
            }
            catch (Exception ex)
            {
                ConsoleUtils.WriteLn(ex);
                return null;
            }
        }

        public static string GetLocalIP(string targetIp)
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            var subnet = targetIp.Substring(0, targetIp.IndexOf('.'));
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork" && ip.ToString().StartsWith(subnet))
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }
    }
}
