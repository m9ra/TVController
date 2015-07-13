using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVControler
{
    class UpnpRequest
    {
        /// <summary>
        /// Pattern for http header used for Upnp requests.
        /// Uses these format parameters: 0-Method, 1-Url, 2-SoapAction header, 3-Content length, 4-Host.
        /// </summary>
        private const string _headerPattern=
@"{0} {1} HTTP/1.1
Cache-Control: no-cache
Connection: Close
Pragma: no-cache
Content-Type: text/xml; charset=""utf-8""
User-Agent: Microsoft-Windows/6.1 UPnP/1.0 Windows-Media-Player-DMC/12.0.7600.16385 DLNADOC/1.50
User-Agent: m9ra-TvControler/1.0 UPnP/1.0 TvControler/1.0 DLNADOC/1.50
SOAPAction: ""{2}""
Content-Length: {3}
Host: {4}

";

        /// <summary>
        /// Pattern for xml request
        /// Uses these format parameters: 0-request body.
        /// </summary>
        private const string _requestBodyPattern =
@"<?xml version=""1.0""?>
<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" SOAP-ENV:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
    <SOAP-ENV:Body>
{0}
    </SOAP-ENV:Body>
</SOAP-ENV:Envelope>";

        private const string _instanceSpecifier =
@"<InstanceID xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""ui4"">0</InstanceID>
<Channel xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""string"">Master</Channel>";

        /// <summary>
        /// Headers of request are stored here.
        /// Uses these format parameters: 0-Host.
        /// </summary>
        string _headers;
        /// <summary>
        /// Content related to headers.
        /// </summary>
        string _content;

        /// <summary>
        /// Create upnp request
        /// </summary>
        /// <param name="url">Url where request will be sent.</param>
        /// <param name="soap">Soap specifier.</param>
        /// <param name="body">Body of upnp request. These format paramerters are available: 0-instance specifier.</param>
        /// <param name="method">Http method.</param>
        public UpnpRequest(string url, string soap, string body, string method = "POST")
        {
            //process avialble format parameters for body.
            var processedBody = body.Replace("{INSTANCE}", _instanceSpecifier);

            //create request content with headers.
            _content=string.Format(_requestBodyPattern,processedBody);
            _headers=string.Format(_headerPattern,method,url,soap,"{0}","{1}");
        }

        /// <summary>
        /// Get http of request according to given host and format parameters
        /// </summary>
        /// <param name="host"></param>
        /// <param name="formatArgs"></param>
        /// <returns></returns>
        public string GetHttp(string host,params object[] formatArgs)
        {
            var processedContent = string.Format(_content, formatArgs);
            var processedHeaders = string.Format(_headers,processedContent.Length, host);            

            return processedHeaders + processedContent;
        }
    }
}
