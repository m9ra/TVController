using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVControler
{
    class HTTPHeaders
    {
        Dictionary<string, string> _headers = new Dictionary<string, string>();
        string _firstLine;

        private HTTPHeaders(string firstLine,params object[] formatArgs)
        {
            _firstLine = string.Format(firstLine, formatArgs);
        }

        public static HTTPHeaders Response(int code, string codeDescription)
        {
            return new HTTPHeaders("HTTP/1.1 {0} {1}", code, codeDescription);
        }

        public static HTTPHeaders Request(string method, string location)
        {
            return new HTTPHeaders("{0} {1} HTTP/1.1", method, location);
        }

        /// <summary>
        /// If value is null, header will be removed
        /// </summary>
        /// <param name="headerName"></param>
        /// <param name="value"></param>
        public void SetHeader(string headerName, string value)
        {
            if (value == null)
                _headers.Remove(headerName);
            else
                _headers[headerName] = value;
        }

        /// <summary>
        /// Create string representing http headers with doble new line
        /// </summary>
        /// <returns></returns>
        public string ToHttp()
        {
            var result = new StringBuilder();

            result.AppendFormat("{0}\r\n", _firstLine);
            foreach (var header in _headers)
            {
                result.AppendFormat("{0}: {1}\r\n", header.Key, header.Value);
            }

            result.Append("\r\n");
            return result.ToString();
        }
    }
}
