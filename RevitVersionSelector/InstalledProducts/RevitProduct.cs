using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using dosymep.AutodeskApps.FileInfo;

using Microsoft.Win32;

namespace RevitVersionSelector.InstalledProducts {
    public class RevitProduct : InstalledProduct {
        public static readonly Guid ProductGuid = new Guid("DF7D485F-B8BA-448E-A444-E6FB1C258912");

        public static readonly string StartProductGuid = "7346B4A";
        public static readonly string FinishProductGuid = "705C0D862004";

        public static readonly string ApplicationFileName = "Revit.exe";
        public static readonly string RevitRegName = @"REVIT-05";
        public static readonly string RevitRegPath = @"SOFTWARE\Autodesk\Revit";

        public static IEnumerable<RevitProduct> GetInstalledProducts() {
            foreach(RevitProduct revitProduct in InstalledProduct.GetInstalledProducts<RevitProduct>(ProductGuid)) {
                string guidValue = revitProduct.Code.ToString();
                if(guidValue.StartsWith(StartProductGuid, StringComparison.CurrentCultureIgnoreCase)
                   && guidValue.EndsWith(FinishProductGuid, StringComparison.CurrentCultureIgnoreCase)) {
                    yield return revitProduct;
                }
            }
        }

        public string RevitVersion => "20" + DisplayVersion.Major;
        public string ApplicationFilePath => Path.Combine(InstallLocation, ApplicationFileName);

        public override string InstallLocation => GetInstallLocation();

        public string GetInstallLocation() {
            if(Directory.Exists(base.InstallLocation)) {
                return base.InstallLocation;
            }

            string defaultPath = @$"C:\Program Files\Autodesk\Revit {RevitVersion}";
            if(Directory.Exists(defaultPath)) {
                return defaultPath;
            }

            string revitRegPath = Path.Combine(RevitRegPath, RevitVersion);
            using(RegistryKey key = Registry.LocalMachine.OpenSubKey(revitRegPath)) {
                var revitRegName = key?.GetSubKeyNames()
                    .FirstOrDefault(item => item.StartsWith(RevitRegName));

                if(string.IsNullOrEmpty(revitRegName)) {
                    return base.InstallLocation;
                }

                using(RegistryKey revitKey = key?.OpenSubKey(revitRegName)) {
                    return (string) (revitKey?.GetValue("InstallationLocation") ?? base.InstallLocation);
                }
            }
        }
    }
}