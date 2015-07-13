using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVControler
{
    class HTTPRequestParser
    {
        public const string Header_Request = ":req";
        public const string Header_Uri = ":uri";
        public const string Header_Method = ":method";

        /// <summary>
        /// Determine if all data has been obtained.
        /// </summary>
        public bool IsComplete { get; private set; }

        public bool IsHeadComplete { get; private set; }

        public bool IsBodyComplete { get; private set; }

        /// <summary>
        /// Determine if error occured during parsing.
        /// </summary>
        public bool IsError { get; private set; }



        string _name = Header_Request;
        string _value = "";
        bool _headersComplete = false;

        string _body = "";
        string _headerPart = "";

        Dictionary<string, string> _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);


        public string Body { get { return _body; } }
        public string HeaderPart { get { return _headerPart; } }

        /// <summary>
        /// Get header according to specified header name.
        /// </summary>
        /// <param name="headerName"></param>
        /// <returns></returns>
        public string GetHeader(string headerName)
        {
            string result;
            _headers.TryGetValue(headerName, out result);
            return result;
        }

        /// <summary>
        /// Add data recieved from client. 
        /// </summary>
        /// <param name="buffer">Buffer which data will be added.</param>        
        public void AddData(string buffer)
        {
            if (buffer == null)
                return;
            for (int i = 0; i < buffer.Length; ++i)
            {
                var zn = buffer[i];
                if (_headersComplete)
                {
                    _body += zn;
                    continue;
                }
                else
                    _headerPart += zn;

                switch (zn)
                {
                    case '\r':
                        break;
                    case '\n':
                        if (_name.Trim() == "")
                            //end of headers section
                            onHeadersComplete();
                        else
                            //end of single header
                            onHeaderComplete();
                        break;
                    case ':':
                        if (_value == null)
                            //switch reading name into reading header name
                            _value = "";
                        else
                            //char is part of value
                            _value += zn;

                        break;
                    default:
                        if (_value == null)
                            _name += zn;
                        else
                            _value += zn;
                        break;
                }
            }

            IsHeadComplete = _headersComplete;
            if (IsHeadComplete)
            {
                int contentLength = 0;
                string contentLengthString;
                if (_headers.TryGetValue("CONTENT-LENGTH", out contentLengthString))
                    contentLength = int.Parse(contentLengthString);

                IsBodyComplete = Encoding.UTF8.GetByteCount(_body) >= contentLength;
            }

            IsComplete = IsHeadComplete && IsBodyComplete;
        }

        private void onHeaderComplete()
        {
            //switch reading new header
            _name = _name.Trim();
            _value = _value.Trim();
            _headers.Add(_name, _value);

            _value = null;
            _name = "";
        }

        private void onHeadersComplete()
        {
            _headersComplete = true;

            //parse request header
            var req = _headers[Header_Request];
            var toks = req.Split(' ');
            _headers[Header_Method] = toks[0];
            _headers[Header_Uri] = toks[1];
        }

    }
}
