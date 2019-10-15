using System;
using System.Diagnostics;
using System.Linq;
using SandS.Common.Pathes;

namespace SandS.Common.Helpers
{
    // ReSharper disable once AllowPublicClass
    public class ExecuteHelper
    {
        public ExecuteHelper(Action<string> logAction)
        {
            this.logAction = logAction;
        }

        private readonly Action<string> logAction;

        public int ExecuteInPowershell(string pureCommand, FilePath log)
        {
            log.Touch();

            var arguments =
                " -NoProfile -ExecutionPolicy ByPass -Command " +
                $"        \"Write-Host $(whoami) ; & {pureCommand} " +
                " | Out-String | Tee-Object '{log}'\" ";

            var info = new ProcessStartInfo("powershell.exe", arguments);
            logAction?.Invoke($"Starting process: 'powershell.exe {arguments}'");

            var process = Process.Start(info);

            if (process is null ||
                process.Id <= 0)
            {
                logAction?.Invoke("Failed to create GUI process from service one");

                throw new Exception("Failed to create GUI process from service one");
            }

            logAction?.Invoke($"powershell started: powershell PID - {process.Id}," +
                              $" command - 'powershell.exe {arguments}'");

            return process.Id;
        }

        public bool IsProcessAlive(int processId)
        {
            return Process.GetProcesses().Any(x => x.Id == processId);
        }
    }
}