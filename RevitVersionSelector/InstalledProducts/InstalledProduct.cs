using System;
using System.Collections.Generic;
using System.Globalization;
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
                    ProductCode = productGuid,
                    DisplayName = GetValue<string>(productCode, "DisplayName"),
                    DisplayVersion = GetVersionValue(productCode, "DisplayVersion"),
                    InstallDate = GetDateTimeValue(productCode, "InstallDate"),
                    InstallSource = GetValue<string>(productCode, "InstallSource"),
                    InstallLocation = GetValue<string>(productCode, "InstallLocation"),
                    Publisher = GetValue<string>(productCode, "Publisher"),
                    Language = GetCultureInfoValue(productCode, "Language"),
                };
            }
        }

        public Guid Code { get; private set; }
        public Guid ProductCode { get; private set; }

        public virtual string DisplayName { get; private set; }
        public virtual Version DisplayVersion { get; private set; }

        public virtual DateTime InstallDate { get; private set; }
        public virtual string InstallSource { get; private set; }
        public virtual string InstallLocation { get; private set; }

        public virtual string Publisher { get; private set; }
        public virtual CultureInfo Language { get; private set; }

        protected static T GetValue<T>(Guid productCode, string propertyName, T defaultValue = default) {
            string subKeyPath = Path.Combine(SubKeyPath, productCode.ToString("B"));
            using(RegistryKey key = Registry.LocalMachine.OpenSubKey(subKeyPath)) {
                return (T) (key?.GetValue(propertyName) ?? defaultValue);
            }
        }

        protected static Version GetVersionValue(Guid productCode, string propertyName) {
            return new Version(GetValue<string>(productCode, propertyName));
        }

        protected static DateTime GetDateTimeValue(Guid productCode, string propertyName) {
            return DateTime.ParseExact(
                GetValue<string>(productCode, propertyName),
                "yyyyMMdd", CultureInfo.CurrentCulture);
        }

        protected static CultureInfo GetCultureInfoValue(Guid productCode, string propertyName,
            CultureInfo defaultValue = default) {
            var cultureCode = GetValue<int?>(productCode, propertyName);
            return cultureCode.HasValue ? CultureInfo.GetCultureInfo(cultureCode.Value) : defaultValue;
        }
    }
}