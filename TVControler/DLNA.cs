using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TVControler
{
    static class DLNA
    {
        private static string _sharedFolder,_networkFolder;

        private static SharedTree _tree;

        public static SharedTree Tree
        {
            get
            {
                if (_tree == null)
                    _tree = new SharedTree(_sharedFolder,_networkFolder );
                return _tree;
            }
        }

        public static void SetFolderBinding( string sharedFolder,string networkFolder)
        {
            _sharedFolder = sharedFolder;
            _networkFolder = networkFolder; 
        }
    }
}
