using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;

namespace TVControler
{
    static class UpnpProtocol
    {
        public const string UUID = "c9f78cd9-06a0-4296-9d30-bdaaa4a981e7";

        /**/
        //============SAMSUNG TV===========
        public static readonly string RenderingControlURL = @"/upnp/control/RenderingControl1";

        public static readonly string AvTransportURL = @"/upnp/control/AVTransport1";

        //=================================
         /**/

        /*
        //============KODI=================
        public static readonly string RenderingControlURL = @"/RenderingControl/7cb36200-883d-1b84-d2d8-04d2b0567f1e/control.xml";

        public static readonly string AvTransportURL = @"/AVTransport/7cb36200-883d-1b84-d2d8-04d2b0567f1e/control.xml";

        //=================================
         * */

        /// <summary>
        /// Upnp Services, uses format parameters: 0-UUID
        /// </summary>
        public static readonly string[] Service_Descriptors = new string[]{
            "upnp:rootdevice",
            "uuid:{0}",          
            "urn:schemas-upnp-org:device:MediaServer:1",            
            "urn:schemas-upnp-org:service:ContentDirectory:1",
            "urn:schemas-upnp-org:service:ConnectionManager:1"
        };

        public const string Request_1 =
@"GET /dmr/SamsungMRDesc.xml HTTP/1.1
Cache-Control: no-cache
Connection: Close
Pragma: no-cache
Accept: text/xml, application/xml
Host: 192.168.1.53:52235
User-Agent: Microsoft-Windows/6.1 UPnP/1.0 Windows-Media-Player-DMC/12.0.7600.16385 DLNADOC/1.50

";
        public const string Request_2 =
@"";

        /// <summary>
        /// Request for setting volume at value specified by {0} parameter - has to be single figure.
        /// </summary>
        public static readonly UpnpRequest SetVolume = new UpnpRequest(
RenderingControlURL,
@"urn:schemas-upnp-org:service:RenderingControl:1#SetVolume",
@"<m:SetVolume xmlns:m=""urn:schemas-upnp-org:service:RenderingControl:1"">
    {INSTANCE}
    <DesiredVolume xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""ui2"">{0}</DesiredVolume>
</m:SetVolume>"
);

        /// <summary>
        /// Request for getting volume value.
        /// </summary>
        public static readonly UpnpRequest GetVolume = new UpnpRequest(
RenderingControlURL,
@"urn:schemas-upnp-org:service:RenderingControl:1#GetVolume",
@"<m:GetVolume xmlns:m=""urn:schemas-upnp-org:service:RenderingControl:1"">
    {INSTANCE}
</m:GetVolume>");


        /// <summary>
        /// Request for getting position of currently played AVTransport.
        /// </summary>
        public static readonly UpnpRequest GetPosition = new UpnpRequest(
AvTransportURL,
@"urn:schemas-upnp-org:service:AVTransport:1#GetPositionInfo",
@"<u:GetPositionInfo xmlns:u=""urn:schemas-upnp-org:service:AVTransport:1"">
    {INSTANCE}
</u:GetPositionInfo>");

        /// <summary>
        /// Request for getting info of currently played AVTransport.
        /// </summary>
        public static readonly UpnpRequest GetTransportInfo = new UpnpRequest(
AvTransportURL,
@"urn:schemas-upnp-org:service:AVTransport:1#GetTransportInfo",
@"<u:GetTransportInfo xmlns:u=""urn:schemas-upnp-org:service:AVTransport:1"">
    {INSTANCE}
</u:GetTransportInfo>");

        /// <summary>
        /// Request for play command on currently played AVTransport. Uses parameter 0-Speed
        /// </summary>
        public static readonly UpnpRequest Play = new UpnpRequest(
AvTransportURL,
@"urn:schemas-upnp-org:service:AVTransport:1#Play",
@"<m:Play xmlns:m=""urn:schemas-upnp-org:service:AVTransport:1"">
    {INSTANCE}
    <Speed xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""string"">1</Speed>
</m:Play>");


        /// <summary>
        /// Request for stop command on currently played AVTransport. Uses parameter 0-Speed
        /// </summary>
        public static readonly UpnpRequest Stop = new UpnpRequest(
AvTransportURL,
@"urn:schemas-upnp-org:service:AVTransport:1#Stop",
@"<m:Stop xmlns:m=""urn:schemas-upnp-org:service:AVTransport:1"">
    {INSTANCE}
    <Speed xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""string"">1</Speed>
</m:Stop>");

        /// <summary>
        /// Request for pause command on currently played AVTransport. Uses parameter 0-Speed
        /// </summary>
        public static readonly UpnpRequest Pause = new UpnpRequest(
AvTransportURL,
@"urn:schemas-upnp-org:service:AVTransport:1#Pause",
@"<m:Pause xmlns:m=""urn:schemas-upnp-org:service:AVTransport:1"">
    {INSTANCE}
    <Speed xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""string"">1</Speed>
</m:Pause>");

        /// <summary>
        /// Seek to position in milliseconds specified by format argument 0.
        /// </summary>
        public static readonly UpnpRequest SeekTo = new UpnpRequest(
AvTransportURL,
@"urn:schemas-upnp-org:service:AVTransport:1#Seek",
@"<u:Seek xmlns:u=""urn:schemas-upnp-org:service:AVTransport:1"">
    {INSTANCE}
    <Unit>REL_TIME</Unit>
    <Target>{0}</Target>
</u:Seek>");

        /// <summary>
        /// Set URI which will be source for AV Transport specified by format argument 0 - URI (http://75.101.165.227:8080/app/iLJy+VD9xyYqv5jtERGBijAeiqUmYWqCFzy4Li6gM0uMzI8pYoRWTxqp+UxEy14ibHGOrLpqJTkjI+WE6Q6lbQ6e2+1X96ToH8lGCv0f4f88M0jxU6S6z4SwC8KOCoMhscRxjOiy4CJVzNNeCGQxpw==.mp4)
        /// </summary>
        public static readonly UpnpRequest SetAVTransportURI = new UpnpRequest(
AvTransportURL,
@"urn:schemas-upnp-org:service:AVTransport:1#SetAVTransportURI",
@"<u:SetAVTransportURI xmlns:u=""urn:schemas-upnp-org:service:AVTransport:1"">
    {INSTANCE}
    <CurrentURI>{0}</CurrentURI>
    <CurrentURIMetaData></CurrentURIMetaData>
</u:SetAVTransportURI>
");


        /// <summary>
        /// Set next URI which will be source for AV Transport specified by format argument 0 - URI (http://75.101.165.227:8080/app/iLJy+VD9xyYqv5jtERGBijAeiqUmYWqCFzy4Li6gM0uMzI8pYoRWTxqp+UxEy14ibHGOrLpqJTkjI+WE6Q6lbQ6e2+1X96ToH8lGCv0f4f88M0jxU6S6z4SwC8KOCoMhscRxjOiy4CJVzNNeCGQxpw==.mp4)
        /// </summary>
        public static readonly UpnpRequest SetNextAVTransportURI = new UpnpRequest(
AvTransportURL,
@"urn:schemas-upnp-org:service:AVTransport:1#SetNextAVTransportURI",
@"<u:SetNextAVTransportURI xmlns:u=""urn:schemas-upnp-org:service:AVTransport:1"">
    {INSTANCE}
    <NextURI>{0}</NextURI>
    <NextURIMetaData></NextURIMetaData>
</u:SetNextAVTransportURI>
");

        public static string GetBrowseResponse(DIDLResult didl)
        {
            var htmlEnc = HTTPProtocol.HtmlEncode(didl.ResultString);
            return string.Format(
@"<?xml version=""1.0""?>
<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" SOAP-ENV:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"">
	<SOAP-ENV:Body>
		<m:BrowseResponse xmlns:m=""urn:schemas-upnp-org:service:ContentDirectory:1"">
			<Result xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""string"">
                {2}
            </Result>
			<NumberReturned xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""ui4"">{0}</NumberReturned>
			<TotalMatches xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""ui4"">{1}</TotalMatches>
			<UpdateID xmlns:dt=""urn:schemas-microsoft-com:datatypes"" dt:dt=""ui4"">0</UpdateID>
		</m:BrowseResponse>
	</SOAP-ENV:Body>
</SOAP-ENV:Envelope>", didl.TotalFounded, didl.TotalFounded, htmlEnc);
        }

        public static string GetUSN(string uuid, string servicename)
        {
            if (servicename.Contains("uuid:"))
                return servicename;

            return "uuid:" + uuid + "::" + servicename;
        }


    }




}
