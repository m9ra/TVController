using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace TVControler
{
    class HTTPResponse
    {
        /// <summary>
        /// Headers to send (include emtpy crlf)
        /// </summary>
        public string Headers { get; private set; }
        /// <summary>
        /// Stream for http content
        /// </summary>
        public ResponseStream Stream { get; private set; }

        private HTTPResponse(HTTPHeaders headers, ResponseStream stream)
        {
            headers.SetHeader("Server", "m9ras Upnp DLNA home media server");
            Headers = headers.ToHttp();
            Stream = stream;
        }

        public static HTTPResponse FromFile(HTTPRequestParser request,string path){
            var headers = createHeaders(request, 200);
            var splited = path.Split('?');
            path = splited[0];
            
            var info = new FileInfo(path);
            if (!info.Exists)
                return FromData(request, "Path "+path+" is unavailable", 500);
            var stream= new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            
            var respStream = createResponseStream(headers, request, stream, info.Length);
            var mime = GetContentType(path);
            headers.SetHeader("Content-Type",mime);
            headers.SetHeader("Accept-Ranges","bytes");
            headers.SetHeader("ContentFeatures.DLNA.ORG","DLNA.ORG_OP=01;DLNA.ORG_FLAGS=01500000000000000000000000000000");
            headers.SetHeader("TransferMode.DLNA.ORG", "Streaming");

            return new HTTPResponse(headers, respStream);
        }

        public static string GetContentType(string fileName)
        {
            string contentType = "application/octetstream";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (registryKey != null && registryKey.GetValue("Content Type") != null)
                contentType = registryKey.GetValue("Content Type").ToString();
            if (contentType != "text/xml")
                return "video/avi";
            return contentType;
        } 

        public static HTTPResponse FromData(HTTPRequestParser request, string data,int code=200)
        {
            return fromData(request, data, code);
        }

        public static HTTPResponse NotFound(HTTPRequestParser request)
        {
            return fromData(request, "Page was not found", 404);
        }

        private static HTTPResponse fromData(HTTPRequestParser request, string data, int code)
        {
            string description="";
            switch (code)
            {
                case 500:
                    description = "Internal server error";
                    break;
                case 404:
                    description = "Not found";
                    break;
            }
            var headers = createHeaders(request, code, description);            
            var respStream = createResponseStream(headers, request, createStream(data),Encoding.UTF8.GetByteCount(data));

            return new HTTPResponse(headers, respStream);
        }

        private static HTTPHeaders createHeaders(HTTPRequestParser request,int code, string codeDescription="")
        {
            var hasRange=request.GetHeader("Range") == null;

            switch (code)
            {
                case 200:
                case 206:
                    code = hasRange ? 200 : 206;
                    if(codeDescription=="")
                        codeDescription = hasRange ? "OK" : "Partial content";
                    break;                
            }

            return HTTPHeaders.Response(code, codeDescription);
        }

        /// <summary>
        /// Create response stream according to data stream and range request. Also inject appropriate range and length headers
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="request"></param>
        /// <param name="dataStream"></param>
        /// <param name="dataTotalLength"></param>
        /// <returns></returns>
        private static ResponseStream createResponseStream(HTTPHeaders headers, HTTPRequestParser request,Stream dataStream ,long dataTotalLength)
        {
            var rangeSelector = request.GetHeader("Range");
            var range=new HTTPRange(dataTotalLength,rangeSelector);

            if (range.IsPartial)           
                //inject range header into response headers
                headers.SetHeader("Content-Range", string.Format("bytes {0}-{1}/{2}",range.FromBytes,range.ToBytes,range.FileLength));

            headers.SetHeader("Content-Length", range.ContentLength.ToString());
            return new ResponseStream(dataStream, range.FromBytes, range.ToBytes);
        }

        private static Stream createStream(string data)
        {            
            var stream = new MemoryStream();            
            var writer = new StreamWriter(stream);
            writer.Write(data);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
