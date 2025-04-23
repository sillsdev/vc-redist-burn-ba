using Microsoft.Win32;

namespace SIL.VCRedistBurnBA
{
    public static class VcRedistHelper
    {
        public static bool IsInstalled(int minMinorVersion = 1)
        {
            const string registryKey = @"SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64";
            using (var key = Registry.LocalMachine.OpenSubKey(registryKey))
            {
                if (key != null)
                {
                    var installed = key.GetValue("Installed");
                    var minorVersion = key.GetValue("Minor");
                    return installed != null && (int)installed == 1 && 
                        minorVersion != null && (int)minorVersion >= minMinorVersion;
                }
            }
            return false;
        }

        public static string DownloadInstaller()
        {
            // TODO: Implement the logic to download the VC++ Redistributable installer.
            return "";
        }
    }
}
