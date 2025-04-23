using System;
using System.IO;
using WixToolset.Mba.Core;

namespace SIL.VCRedistBurnBA
{
    public class CustomBootstrapperApplication : BootstrapperApplication
    {
        private const int kDefaultMinMinorVersion = 38;
        private int _minMinorVersion;

        public CustomBootstrapperApplication(IEngine engine) : base(engine)
        {
        }

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

                var pathToInstaller = VcRedistHelper.DownloadInstaller();
                if (pathToInstaller == null)
                {
                    engine.Log(LogLevel.Error, "Failed to download VC++ Redistributable installer.");
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
