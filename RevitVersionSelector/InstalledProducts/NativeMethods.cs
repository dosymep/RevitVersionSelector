using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RevitVersionSelector.InstalledProducts {
    internal class NativeMethods {
        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        private static extern uint MsiEnumClients(
            string szComponent,
            uint iProductIndex,
            string lpProductBuf);

        public static IEnumerable<Guid> MsiEnumClients(Guid productGuid) {
            uint index = 0;
            string result = new('0', 38);
            while(MsiEnumClients(productGuid.ToString("B"), index++, result) == 0) {
                yield return new Guid(result);
            }
        }
    }
}