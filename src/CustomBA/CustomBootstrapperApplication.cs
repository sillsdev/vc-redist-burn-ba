using System;
using System.IO;
using WixToolset.Mba.Core;

namespace SIL.VCRedistBurnBA
{
    public class CustomBootstrapperApplication(IEngine engine) : BootstrapperApplication(engine)
    {
        private const int kDefaultMinMinorVersion = 38;
        private int _minMinorVersion;

        protected override void Run()
        {
            engine.Log(LogLevel.Standard, "Starting Custom Bootstrapper Application...");

            try
            {
                _minMinorVersion = (int)engine.GetVariableNumeric("MinMinorVersion");
            }
            catch
            {
                engine.Log(LogLevel.Error, "Failed to retrieve MinMinorVersion parameter. Using default value.");
                _minMinorVersion = kDefaultMinMinorVersion;
            }

            if (!VcRedistHelper.IsInstalled(_minMinorVersion))
            {
                engine.Log(LogLevel.Standard, $"VC++ Redistributable 14.{_minMinorVersion} or " +
                                              "later not detected. Proceeding with installation.");

                string tempDir = Path.Combine(Path.GetTempPath(), "VC_Redist_Download");
                Directory.CreateDirectory(tempDir);
                string pathToInstaller;
                try
                {
                     pathToInstaller = VcRedistHelper.DownloadInstallerAsync(tempDir).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    engine.Log(LogLevel.Error, $"Failed to download VC++ Redistributable installer. Error: {ex.Message}");
                    engine.SetVariableString("VcRedistInstallerPath", "", false);
                    engine.Quit(1);
                    return;
                }

                engine.SetVariableString("VcRedistInstallerPath", pathToInstaller, false);
            }
            else
            {
                engine.Log(LogLevel.Standard, "VC++ Redistributable already installed. " +
                                              "Skipping installation.");
                engine.SetVariableString("VcRedistInstallerPath", "", false);
            }

            engine.Quit(0);
        }
    }

}
