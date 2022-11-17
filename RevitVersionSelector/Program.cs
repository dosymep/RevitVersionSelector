using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using dosymep.Revit.FileInfo;

using Microsoft.WindowsAPICodePack.Dialogs;

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
                Application.EnableVisualStyles();

                string revitFilePath = args[0];
                Console.WriteLine(@"Args[0]: {0}", revitFilePath);

                try {
                    var revitFileInfo = new RevitFileInfo(revitFilePath);
                    string format = revitFileInfo.BasicFileInfo.AppInfo.Format;
                    Console.WriteLine(@"Format: {0}", format);

                    RevitProduct revitProduct = RevitProduct.GetInstalledProducts()
                        .FirstOrDefault(item => item.RevitVersion.Equals(format));
                    
                    if(revitProduct == null) {
                        var latestVersions = RevitProduct.GetInstalledProducts()
                            .Where(item => string.Compare(item.RevitVersion, format, StringComparison.Ordinal) > 0)
                            .ToArray();
                    
                        OpenRevitFile(revitFileInfo, latestVersions);
                    } else {
                        OpenRevitFile(revitFileInfo, revitProduct);
                    }
                } catch(Exception ex) {
                    ShowMessage(revitFilePath, ex.Message);
                    Console.WriteLine(@"Exception: {0}", ex);
                }
            } else {
                Console.WriteLine(@"Wrong Args: {0}", string.Join(", ", args));
            }
        }

        private static void OpenRevitFile(RevitFileInfo revitFileInfo, params RevitProduct[] revitProducts) {
            string format = revitFileInfo.BasicFileInfo.AppInfo.Format;
            if(revitProducts.Length == 0) {
                ShowMessage(revitFileInfo.ModelPath,
                    string.Format(StringResources.MessageBoxContent, format));
                return;
            }

            if(revitProducts.Length == 1) {
                RevitProduct revitProduct = revitProducts[0];
                if(!File.Exists(revitProduct.ApplicationFilePath)) {
                    throw new InvalidOperationException(
                        string.Format(StringResources.ExceptionApplicationFile, revitProduct.ApplicationFilePath));
                }

                Console.WriteLine(@"Application File Path: {0}", revitProduct.ApplicationFilePath);
                Process.Start(revitProduct.ApplicationFilePath, $"\"{revitFileInfo.ModelPath}\"");
                return;
            }

            var taskDialog = new TaskDialog() {
                Cancelable = true,
                HyperlinksEnabled = true,
                Icon = TaskDialogStandardIcon.Warning,
                StartupLocation = TaskDialogStartupLocation.CenterScreen,
                Text = string.Format(StringResources.TaskDialogText, format),
                Caption = StringResources.MessageBoxTitle,
                FooterText = "by <a href=\"https:\\\\github.com\\dosymep\">dosymep</a>",
                FooterIcon = TaskDialogStandardIcon.Information,
                InstructionText = StringResources.TaskDialogHeading
            };

            taskDialog.HyperlinkClick += (s, e) => Process.Start(e.LinkText);
            var buttons = CreateButtons(taskDialog, revitFileInfo, revitProducts).ToArray();
            buttons[buttons.Length - 2].Default = true;
            foreach(TaskDialogCommandLink taskDialogCommandLink in buttons) {
                taskDialog.Controls.Add(taskDialogCommandLink);
            }

            taskDialog.Show();
        }

        private static IEnumerable<TaskDialogCommandLink> CreateButtons(TaskDialog taskDialog,
            RevitFileInfo revitFileInfo,
            params RevitProduct[] revitProducts) {
            foreach(RevitProduct revitProduct in revitProducts.Skip(revitProducts.Length - 3)) {
                yield return CreateButton(taskDialog, revitFileInfo, revitProduct);
            }

            var exitCommand = new TaskDialogCommandLink(StringResources.TaskDialogExitButtonName,
                StringResources.TaskDialogExitButtonName);
            exitCommand.Click += (s, e) => taskDialog.Close();

            yield return exitCommand;
        }

        private static TaskDialogCommandLink CreateButton(TaskDialog taskDialog, RevitFileInfo revitFileInfo,
            RevitProduct revitProduct) {
            var button = new TaskDialogCommandLink(revitProduct.DisplayName, revitProduct.DisplayName);
            button.Click += (s, e) => {
                taskDialog.Close();
                OpenRevitFile(revitFileInfo, revitProduct);
            };

            return button;
        }

        private static void ShowMessage(string revitFilePath, string messageBoxContent) {
            MessageBox.Show(messageBoxContent,
                StringResources.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}