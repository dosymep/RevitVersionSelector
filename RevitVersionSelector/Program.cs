using System;
using System.Diagnostics;
using System.Linq;

using dosymep.Revit.FileInfo;

using RevitVersionSelector.InstalledProducts;

namespace RevitVersionSelector {
    internal class Program {
        public static void Main(string[] args) {
            if(args.Length == 1) {
                string revitFilePath = args[0];
                var revitFileInfo = new RevitFileInfo(revitFilePath);

                string format = revitFileInfo.BasicFileInfo.AppInfo.Format;
                RevitProduct revitProduct = RevitProduct.GetInstalledProducts()
                    .FirstOrDefault(item => item.RevitVersion.Equals(format));

                if(revitProduct == null) {
                    Process.Start("explorer.exe", revitFilePath);
                } else {
                    Process.Start(revitProduct.ApplicationFilePath, revitFilePath);
                }
            }
        }
    }
}