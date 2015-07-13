using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace TVControler
{
    enum ItemType { file, folder }

    class TreeItem
    {
        public string Path;
        public ItemType Type;
        public int Children;
    }

    class DIDLResult
    {
        public readonly string ResultString;
        public readonly int NumberReturned;
        public readonly int TotalFounded;
        public DIDLResult(string resultString, int displayed, int totalFounded)
        {
            ResultString = resultString;
            TotalFounded = totalFounded;
            NumberReturned = displayed;
        }
    }

    class SharedTree
    {
        string _sharedRoot;
        string _networkRoot;
        Dictionary<string, string> _pathLookup = new Dictionary<string, string>();

        public SharedTree(string rootFolder, string sharedRoot)
        {
            _sharedRoot = rootFolder;
            _networkRoot = sharedRoot;
        }

        /// <summary>
        /// Returns didl formated metadata from object with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal DIDLResult ToDIDL_Lite_Meta(string id)
        {
            if (!id.Contains(_sharedRoot))
                id = _sharedRoot + @"\" + id;

            var isFolder = Directory.Exists(id);

            var item = new TreeItem();
            item.Type =isFolder?ItemType.folder:ItemType.file;
            item.Path = id;
            item.Children = isFolder?Directory.GetDirectories(id).Length:0;


            var didl = encapsulateWithDIDL(item);
            return new DIDLResult(didl, 1, 1);
        }

        /// <summary>
        /// Return DIDL Lite representation of branch from current browsePoint
        /// </summary>
        /// <param name="browsePoint"></param>
        /// <returns></returns>
        public DIDLResult ToDIDL_Lite(string browsePoint, int listStart, int maxItems)
        {
            if (!browsePoint.Contains(_sharedRoot))
                browsePoint = _sharedRoot + @"\" + browsePoint;

            var items = getItems(browsePoint);
            if (maxItems == 0)
                maxItems = items.Length;
            var selected = items.Skip(listStart).Take(maxItems).ToArray();

            var totalFound = items.Length;
            var returnedFound = selected.Length;

            var didl = encapsulateWithDIDL(selected);
            return new DIDLResult(didl, totalFound, returnedFound);
        }

        private string encapsulateWithDIDL(params TreeItem[] selected)
        {
            var outputBuffer = new StringBuilder();
        
            outputBuffer.Append(
@"<DIDL-Lite xmlns=""urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/"" xmlns:dc=""http://purl.org/dc/elements/1.1/"" xmlns:upnp=""urn:schemas-upnp-org:metadata-1-0/upnp/"">"
);
            
            foreach (var item in selected)
                outputBuffer.Append(ToDIDL_Lite(item));

            outputBuffer.Append("</DIDL-Lite>");
            return outputBuffer.ToString();
        }

        private string upnpObjectType(string filePath)
        {
            var ext = Path.GetExtension(filePath);
            ext = ext.Replace(".","").ToLower();
            switch (ext)
            {
                case "wmv":
                case "avi":
                case "mpg":
                case "mp4":
                case "mpeg":
                case "mkv":
                case "ts":
                    return "object.item.videoItem";
                case "bmp":
                case "jpg":
                case "jpeg":
                case "png":
                    return "object.item.imageItem";
                case "mp3":
                case "ogg":
                case "wav":
                case "wma":
                    return "object.item.audioItem";
            }
            //not found
            return "";
        }


        private string ToDIDL_Lite(TreeItem item)
        {
            //0-path, 1-parentFolderPath, 2-item name, 3-children count,4-item type,5-shared path,6-upnp object type
            string formatPattern = "";
            switch (item.Type)
            {
                case ItemType.folder:
                    formatPattern =
@"<container id=""{0}"" parentID=""{1}"" restricted=""1"">
	<dc:title>{2}</dc:title>
    <upnp:class>object.container</upnp:class>
</container>";
                    break;
                case ItemType.file:
                    formatPattern =
@"<item id=""{0}"" parentID=""{1}"" restricted=""1"">
      <dc:title>{2}</dc:title>
      <dc:creator>Jmeno autora - nezname</dc:creator>    
      <upnp:class>{6}</upnp:class>
      <upnp:genre>Nazev zanru - neznamy</upnp:genre>
      <upnp:artist role=""Performer"">Jmeno umelce - nezname</upnp:artist>
      <upnp:album>Nazev alba/serie - nezname</upnp:album>
      <dc:date>2012-06-23</dc:date>
      <upnp:actor>Jmeno herce - nezname</upnp:actor>
	  <res protocolInfo=""http-get:*:video/vnd.dlna.mpeg-tts:DLNA.ORG_PN=AVC_TS_HD_50_AC3_T;DLNA.ORG_OP=01;DLNA.ORG_FLAGS=01500000000000000000000000000000"">{5}</res>
   </item>";
                    break;
            }

            var info=Directory.GetParent(item.Path);
            var parentPath =info.FullName ;
            if (parentPath == _sharedRoot)
                parentPath = "0";
          
            var itemName = Path.GetFileName(item.Path);
            var networkPath = _networkRoot + "/" + item.Path.Substring(_sharedRoot.Length);
            var upnpType = upnpObjectType(item.Path);

            return string.Format(formatPattern, item.Path, parentPath, itemName, item.Children, item.Type, networkPath.Replace(" ","%20"),upnpType);
        }

        private TreeItem[] getItems(string path)
        {
            if (!Directory.Exists(path))
                path = _sharedRoot;
            
            var subdirs = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);

            var result = new List<TreeItem>();

            foreach (var subdir in subdirs)
            {
                var item = new TreeItem();
                item.Type = ItemType.folder;
                item.Path = subdir;
                item.Children = Directory.GetDirectories(subdir).Length;

                result.Add(item);
            }

            foreach (var file in files)
            {
                var item = new TreeItem();
                item.Type = ItemType.file;
                item.Path = file;
                item.Children = 0;

                result.Add(item);
            }

            return result.ToArray();
        }

        internal string GetFilePath(string alias)
        {
            if (_pathLookup.ContainsKey(alias))
                return _pathLookup[alias];
            return _sharedRoot + "/" + alias;
        }


        public void SetLookup(string alias, string path)
        {
            _pathLookup[alias] = path;
        }
    }
}
