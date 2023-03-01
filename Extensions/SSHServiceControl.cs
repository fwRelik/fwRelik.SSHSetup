using fwRelik.SSHSetup.Enums;
using fwRelik.Terminal;
using fwRelik.Terminal.Extensions;

namespace fwRelik.SSHSetup.Extensions
{
    /// <summary>
    /// Allows you to manage the SSH service.
    /// </summary>
    public class SSHServiceControl
    {
        private readonly string _serviceName = "sshd";

        /// <summary>
        /// Service state for this session.
        /// </summary>
        public SSHServiceState ServiceStatus { get; private set; } = SSHServiceState.Stopped;

        /// <summary>
        /// Display service start mode.
        /// </summary>
        /// <remarks>
        /// Service start mode independent of this application.
        /// The value can be changed with <see cref="Set"/>
        /// </remarks>
        public SSHServiceStartupType ServiceStartupType { get; private set; }

        /// <summary>
        /// Sets the service to the given value. Default value is <see cref="SSHServiceStartupType.Automatic"/>.
        /// </summary>
        /// <remarks>
        /// The name of the "sshd" service, currently cannot be changed.
        /// </remarks>
        /// <param name="startupType">Accepts the state of the service <see cref="SSHServiceStartupType"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public void Set(SSHServiceStartupType startupType = SSHServiceStartupType.Automatic)
        {
            string command = $"{CmdletType.Set}-Service -Name {_serviceName} -DisplayName {_serviceName} -StartupType {startupType}";
            TerminalClient.Command(command, process =>
            {
                if (process.Error != null)
                {
                    ServiceStartupType = SSHServiceStartupType.Disabled;
                    throw new ArgumentException(process.Error.Message);
                }
                ServiceStartupType = startupType;
            });
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        /// <remarks>
        /// If the status of the <see cref="SSHServiceStartupType.Disabled"/> 
        /// service will throw out the exception, since the deactivated service cannot be launched.
        /// </remarks>
        /// <exception cref="ArgumentException"></exception>
        public void Start()
        {
            StatusHandler();
            string command = $"{CmdletType.Start}-Service {_serviceName}";
            TerminalClient.Command(command, process =>
            {
                if (process.Error != null)
                {
                    ServiceStatus = SSHServiceState.Error;
                    throw new ArgumentException(process.Error.Message);
                }
                ServiceStatus = SSHServiceState.Running;
            });
        }

        /// <summary>
        /// Restart the service.
        /// </summary>
        /// <remarks>
        /// If the status of the <see cref="SSHServiceStartupType.Disabled"/> 
        /// service will throw out the exception, since the deactivated service cannot be restarting.
        /// </remarks>
        /// <exception cref="ArgumentException"></exception>
        public void Restart()
        {
            StatusHandler();
            string command = $"{CmdletType.Restart}-Service {_serviceName}";
            TerminalClient.Command(command, process =>
            {
                if (process.Error != null)
                {
                    ServiceStatus = SSHServiceState.Error;
                    throw new ArgumentException(process.Error.Message);
                }
                ServiceStatus = SSHServiceState.Running;
            });
        }

        /// <summary>
        /// Stop the service.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Stop()
        {
            StatusHandler();
            string command = $"{CmdletType.Stop}-Service {_serviceName}";
            TerminalClient.Command(command, process =>
            {
                if (process.Error != null)
                {
                    ServiceStatus = SSHServiceState.Error;
                    throw new ArgumentException(process.Error.Message);
                }
                ServiceStatus = SSHServiceState.Stopped;
            });
        }

        /// <summary>
        /// Receives the state of the service.
        /// </summary>
        /// <returns><see cref="SSHServiceState"/></returns>
        /// <exception cref="ArgumentException"></exception>
        public SSHServiceState GetServiceStatus()
        {
            string command = $"{CmdletType.Get}-Service {_serviceName} | Select-Object -ExpandProperty Status";
            TerminalClient.Command(command, process =>
            {
                if (process.Error != null)
                {
                    ServiceStatus = SSHServiceState.Error;
                    throw new ArgumentException(process.Error.Message);
                }

                bool result = TerminalParser.CheckValue(
                    stdOut: process.StdOut,
                    keyword: SSHServiceState.Running.ToString(),
                    stateValue: SSHServiceState.Running.ToString()
                );

                ServiceStatus = result ? SSHServiceState.Running : SSHServiceState.Stopped;
            });

            return ServiceStatus;
        }

        // Temporarily not implemented because the compensating action requires data not available for this throw.
        private void Remove()
        {
            string command = $"{CmdletType.Remove}-Service -Name {_serviceName}";
            TerminalClient.Command(command, process =>
            {
                if (process.Error != null) throw new ArgumentException(process.Error.Message);
            });
        }

        /// <summary>
        /// Checking the state of the service.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private void StatusHandler()
        {
            if (ServiceStartupType == SSHServiceStartupType.Disabled)
                throw new ArgumentException($"This command cannot be executed when the Service is {ServiceStartupType}");
        }
    }
}
