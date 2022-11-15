using System;
using System.Collections.Generic;
using System.IO;

namespace RevitVersionSelector.InstalledProducts {
    public class RevitProduct : InstalledProduct {
        public static readonly Guid ProductGuid = new Guid("DF7D485F-B8BA-448E-A444-E6FB1C258912");

        public static readonly string StartProductGuid = "7346B4A";
        public static readonly string FinishProductGuid = "705C0D862004";

        public static readonly string ApplicationFileName = "Revit.exe";

        public static IEnumerable<RevitProduct> GetInstalledProducts() {
            foreach(RevitProduct revitProduct in InstalledProduct.GetInstalledProducts<RevitProduct>(ProductGuid)) {
                string guidValue = revitProduct.Code.ToString();
                if(guidValue.StartsWith(StartProductGuid, StringComparison.CurrentCultureIgnoreCase)
                   && guidValue.EndsWith(FinishProductGuid, StringComparison.CurrentCultureIgnoreCase)) {
                    yield return revitProduct;
                }
            }
        }

        public string RevitVersion => "20" + DisplayVersion.Split('.')[0];
        public string ApplicationFilePath => Path.Combine(InstallLocation, ApplicationFileName);
    }
}