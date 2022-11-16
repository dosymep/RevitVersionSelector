using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

using dosymep.Revit.FileInfo;

using RevitVersionSelector.InstalledProducts;
using RevitVersionSelector.Resources;

namespace RevitVersionSelector {
    internal class Program {
        public static void Main(string[] args) {
            try {
                StartRevit(args);
            } catch(Exception ex) {
                Console.WriteLine(@"Exception: {0}", ex);
            }
        }

        private static void StartRevit(string[] args) {
            if(args.Length == 1) {
                string revitFilePath = args[0];
                Console.WriteLine(@"Args[0]: {0}", revitFilePath);

                try {
                    var revitFileInfo = new RevitFileInfo(revitFilePath);
                    string format = revitFileInfo.BasicFileInfo.AppInfo.Format;
                    Console.WriteLine(@"Format: {0}", format);

                    RevitProduct revitProduct = RevitProduct.GetInstalledProducts()
                        .FirstOrDefault(item => item.RevitVersion.Equals(format));

                    if(revitProduct == null) {
                        ShowMessage(revitFilePath,
                            string.Format(StringResources.MessageBoxContent, format));
                    } else {
                        if(!File.Exists(revitProduct.ApplicationFilePath)) {
                            throw new InvalidOperationException(
                                string.Format(StringResources.ExceptionApplicationFile,
                                    revitProduct.ApplicationFilePath));
                        }

                        Console.WriteLine(@"Application File Path: {0}", revitProduct.ApplicationFilePath);
                        Process.Start(revitProduct.ApplicationFilePath, $"\"{revitFilePath}\"");
                    }
                } catch(Exception ex) {
                    ShowMessage(revitFilePath, ex.Message);
                    Console.WriteLine(@"Exception: {0}", ex);
                }
            } else {
                Console.WriteLine(@"Wrong Args: {0}", string.Join(", ", args));
            }
        }

        private static void ShowMessage(string revitFilePath, string messageBoxContent) {
            MessageBox.Show(messageBoxContent,
                StringResources.MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}