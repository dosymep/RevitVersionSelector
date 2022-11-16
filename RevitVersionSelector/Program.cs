﻿using System;
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
            if(args.Length == 1) {
                string revitFilePath = args[0];
                try {
                    var revitFileInfo = new RevitFileInfo(revitFilePath);
                    string format = revitFileInfo.BasicFileInfo.AppInfo.Format;
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

                        Process.Start(revitProduct.ApplicationFilePath, $"\"{revitFilePath}\"");
                    }
                } catch(Exception ex) {
                    ShowMessage(revitFilePath, ex.ToString());
                }
            }
        }

        private static void ShowMessage(string revitFilePath, string messageBoxContent) {
            var result = MessageBox.Show(messageBoxContent,
                StringResources.MessageBoxTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(result == MessageBoxResult.Yes) {
                Process.Start("explorer.exe", revitFilePath);
            }
        }
    }
}