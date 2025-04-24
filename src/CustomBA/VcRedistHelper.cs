using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Win32;
using static System.Net.Http.HttpCompletionOption;

namespace SIL.VCRedistBurnBA
{
    public static class VcRedistHelper
    {
        private const string kDownloadUrl = "https://aka.ms/vs/17/release/vc_redist.x64.exe";
        private const string kInstallerFileName = "vc_redist.x64.exe";

        public static bool IsInstalled(int minMinorVersion = 1)
        {
            const string registryKey = @"SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64";
            using var key = Registry.LocalMachine.OpenSubKey(registryKey);
            if (key != null)
            {
                var installed = key.GetValue("Installed");
                var minorVersion = key.GetValue("Minor");
                return installed != null && (int)installed == 1 && 
                       minorVersion != null && (int)minorVersion >= minMinorVersion;
            }

            return false;
        }

        public static async Task<string> DownloadInstallerAsync(string tempDirectory)
        {
            var exePath = Path.Combine(tempDirectory, kInstallerFileName);

            // Download the redistributable
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(kDownloadUrl, ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            using var fs = new FileStream(exePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await responseStream.CopyToAsync(fs);

            return exePath;
        }
    }
}
