using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanonPhotoBooth.csFiles
{
    public static class Folders
    {
        public static string PhotoBoothBaseDir { get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PhotoBooth"); }

        public static string Deleted { get => "Deleted"; }

        public static string Photos { get => "Photos"; }

        public static string ShowTemp { get => "ShowTemp"; }

        public static string Static { get => "Static"; }

        public static string Temp { get => "Temp"; }
    }
}
