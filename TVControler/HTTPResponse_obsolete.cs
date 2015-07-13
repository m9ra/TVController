using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace TVControler
{    
   /* class HTTPResponse_obsolete
    {
        /// <summary>
        /// Headers to send (include emtpy crlf)
        /// </summary>
        public string Headers { get; private set; }
        /// <summary>
        /// Stream for http content
        /// </summary>
        public ResponseStream Stream { get; private set; }

        public HTTPResponse_obsolete(string range, string path)
        {
            var info = new FileInfo(path);
            var parsedRange = new HTTPRange(info.Length, range);
            var read = new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);

            var pattern = HTTPProtocol.Headers_OKContent;
            if (parsedRange.IsPartial)
                pattern = HTTPProtocol.Headers_PartialContent;

            var date = DateTime.Now.ToUniversalTime().ToString("r");
            var mime = GetContentType(path);
//          mime = "video/avi";

            Headers = string.Format(pattern, parsedRange.ContentLength, parsedRange.FromBytes, parsedRange.ToBytes,parsedRange.FileLength,date,mime);
            Stream = new ResponseStream(read,parsedRange.FromBytes,parsedRange.ToBytes);            
        }

        private string GetContentType(string fileName)
        {
            string contentType = "application/octetstream";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (registryKey != null && registryKey.GetValue("Content Type") != null)
                contentType = registryKey.GetValue("Content Type").ToString();
            return contentType;
        } 
    }*/

    class ResponseStream{
        /// <summary>
        /// Determine if is stream end.
        /// </summary>
        public bool End { get { return _currOffset >= _to; } }
        Stream _stream;

        long _from, _to,_currOffset;

        public ResponseStream(Stream stream, long from, long to)
        {
            _stream = stream;
            _currOffset=_from = from;
            _to = to;

            stream.Seek(_from, SeekOrigin.Begin);
        }
        public int GetChunk(byte[] buffer)
        {
            var shift=_stream.Read(buffer, 0, buffer.Length);

            _currOffset+=shift;
            if (_currOffset > _to)
            {

                //NECHAPU return (int)(_currOffset - _to);
                //RADEJI 
                _currOffset = _to;
            }

            return shift;
        }
    }

    class HTTPRange
    {
        public bool IsPartial { get; private set; }
        public long FromBytes { get; private set; }
        public long ToBytes { get; private set; }
        public long FileLength { get; private set; }
        public long ContentLength{get;private set;}

        public HTTPRange(long fileLength, string range)
        {
            IsPartial = range != null;

            ContentLength=FileLength = fileLength;
            FromBytes = 0;
            ToBytes = FileLength-1;

            if (range == null)
                //no range specifiers
                return;

            var borders = new long[2];
            range = range.Replace("bytes=", "");
            var toks = range.Split('-');
            if (toks[0] == "")
                borders[0] = fileLength-long.Parse(toks[1])-1;
            else 
                borders[0]=long.Parse(toks[0]);

            if (toks[1] == "")
                borders[1] = fileLength-1;
            else 
                borders[1]=long.Parse(toks[1]);

            FromBytes = borders[0];
            ToBytes = borders[1];
            ContentLength= ToBytes - FromBytes+1;
        }
    }

    /*  
GET /WMPNSSv4/3727156202/0_e0E5RkUxMTFFLUZBRkMtNDM4Ri1BQTkwLTQ4NzNFQTQ0MEI4NH0uMC44.avi HTTP/1.0
Range: bytes=2048-
getcontentFeatures.dlna.org: 1
transferMode.dlna.org: Streaming
Host: 192.168.1.137:10243
HTTP/1.1 206 Partial Content
Content-Length: 804365824
Content-Type: video/avi
Last-Modified: Sun, 15 Jan 2012 20:03:12 GMT
Server: Microsoft-HTTPAPI/2.0
Accept-Ranges: bytes
ContentFeatures.DLNA.ORG: DLNA.ORG_OP=01;DLNA.ORG_FLAGS=01500000000000000000000000000000
Content-Range: bytes 2048-804367871/804367872
TransferMode.DLNA.ORG: Streaming
Date: Mon, 11 Jun 2012 17:23:05 GMT
Connection: close

00db..................... ......g.Z. Q.....DivX503b1393p....XviD0041.....`..q.. 
     */
}
