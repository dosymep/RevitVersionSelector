using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Win32;

namespace RevitVersionSelector.InstalledProducts {
    public abstract class InstalledProduct {
        public static readonly string SubKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        public static IEnumerable<T> GetInstalledProducts<T>(Guid productGuid)
            where T : InstalledProduct, new() {
            foreach(Guid productCode in NativeMethods.MsiEnumClients(productGuid)) {
                yield return new T() {
                    Code = productCode,
                    DisplayName = GetValue<string>(productCode, "DisplayName"),
                    DisplayVersion = GetValue<string>(productCode, "DisplayVersion"),
                    InstallLocation = GetValue<string>(productCode, "InstallLocation")
                };
            }
        }

        public Guid Code { get; private set; }
        public string DisplayName { get; private set; }
        public string DisplayVersion { get; private set; }
        public string InstallLocation { get; private set; }

        protected static T GetValue<T>(Guid productCode, string propertyName, T defaultValue = default) {
            string subKeyPath = Path.Combine(SubKeyPath, productCode.ToString("B"));
            using(RegistryKey key = Registry.LocalMachine.OpenSubKey(subKeyPath)) {
                return (T) (key?.GetValue(propertyName) ?? defaultValue);
            }
        }
    }
}