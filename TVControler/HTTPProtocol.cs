using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVControler
{
    class HTTPProtocol
    {
        /*
        /// <summary>
        /// Uses format parameters: 0-Content length, 1-From bytes, 2-To bytes,3-File length, 4-Date, 5-Mime
        /// </summary>
        public const string Headers_PartialContent =
@"HTTP/1.1 206 Partial Content
Content-Length: {0}
Content-Type: {5}
Last-Modified: Fri, 13 Jul 2012 11:07:07 GMT
Server: Microsoft-HTTPAPI/2.0
Accept-Ranges: bytes
ContentFeatures.DLNA.ORG: DLNA.ORG_OP=01;DLNA.ORG_FLAGS=01500000000000000000000000000000
Content-Range: bytes {1}-{2}/{3}
TransferMode.DLNA.ORG: Streaming
Date: {4}
Connection: close

";

        /// <summary>
        /// Uses format parameters: 0-Content length,4-Date, 5-Mime
        /// </summary>
        public const string Headers_OKContent =
@"HTTP/1.1 200 OK
Content-Length: {0}
Content-Type: {5}
Last-Modified: Fri, 13 Jul 2012 11:07:07 GMT
Server: Microsoft-HTTPAPI/2.0
Accept-Ranges: bytes
ContentFeatures.DLNA.ORG: DLNA.ORG_OP=01;DLNA.ORG_FLAGS=01500000000000000000000000000000
Content-Range: bytes {1}-{2}/{3}
TransferMode.DLNA.ORG: Streaming
Date: {4}
Connection: close

";*/





        /// <summary>
        /// Uses format parameters: 0-Service name, 1-Description location XML, 2-UUID, 3-USN (uuid:{2}::{0})
        /// </summary>
        public const string Headers_NOTIFY_alive =
@"NOTIFY * HTTP/1.1
HOST: 239.255.255.250:1900
CACHE-CONTROL: max-age = 1800
LOCATION: {1}
NT: {0}
NTS: ssdp:alive
USN: {3}
SERVER: m9ras upnp server
CONTENT-LENGTH: 0

";

        /// <summary>
        /// Uses format parameters: 0-Service name, 1-UUID
        /// </summary>
        public const string Headers_NOTIFY_byebye =
@"NOTIFY * HTTP/1.1
HOST: 239.255.255.250:1900
NTS: ssdp:byebye
USN: uuid:{1}::{0}
NT: {0}
CONTENT-LENGTH: 0

";
        /// <summary>
        /// Uses format parameters: 0-Service location, 1-Search string, 2-USN
        /// </summary>
        public const string Headers_SEARCH_ok =
@"HTTP/1.1 200 OK
CACHE-CONTROL: max-age = 1800
EXT: 
LOCATION: {0}
SERVER: m9ras upnp server
ST: {1}
USN: {2}
CONTENT-LENGTH: 0

";

        private static HashSet<char> _convertable = new HashSet<char>(new Char[] { '<', '>', '\'' });
        private static Dictionary<char, string> _convert = new Dictionary<char, string>()
        {
            {'<',"lt"},
            {'>',"gt"},
      //      {'"',"quot"},
            {'\'',""},            
        };
        public static string HtmlEncode(string text)
        {
       /*     StringBuilder result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

            foreach (char c in text)
            {
                int value = Convert.ToInt32(c);
                if (value > 127 || _convertable.Contains(c))
                {
                    var conversion=_convert.ContainsKey(c)?_convert[c]:"#{0}";
                    result.AppendFormat('&'+conversion+';', value);
                }
                else
                    result.Append(c);
            }

            return result.ToString();*/

            return text.Replace("<", "&lt;").Replace(">","&gt;");
        }

        public static string URLDecode(string url)
        {
          //  StringBuilder result = new StringBuilder();
            var bytes = new List<byte>();
            for (int i = 0; i < url.Length; ++i)
            {
                var ch = url[i];

                var code = (byte)ch;
                switch (ch)
                {
                    case '%':
                        var hexCode = url.Substring(i + 1, 2);
                        var dec=Convert.ToInt32(hexCode, 16);
                        code = (byte)dec; 
                 //       result.Append((char)dec);
                        i += 2;
                        break;
                    default:
                    //    result.Append(ch);
                        break;
                }

                bytes.Add(code);
            }

            var result=Encoding.UTF8.GetString(bytes.ToArray());
            return result;
         //   return result.ToString();
        }


     /*   public static string ResponseOK(HTTPRequestParser parser, string data)
        {
            var pattern = Headers_OKContent;
            var date = DateTime.Now.ToUniversalTime().ToString("r");

            var headers = string.Format(pattern,data.Length ,0,data.Length,data.Length, date, "text/xml");

            if (parser.GetHeader(":method") == "HEAD")
                return headers;

            return headers + data;
        }*/


    }
}
