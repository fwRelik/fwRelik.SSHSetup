using fwRelik.SSHSetup.Enums;
using fwRelik.Terminal;
using fwRelik.Terminal.CallbackArgs;
using fwRelik.Terminal.Extensions;

namespace fwRelik.SSHSetup.Extensions
{
    /// <summary>
    /// Allows you to manage the required Packages.
    /// </summary>
    public class PackageControl
    {
        private readonly string _getPackagesCommand = "Get-WindowsCapability -Online | Where-Object Name -Like 'OpenSSH.*'";
        private readonly string _requiredPackageStatus = "Installed";
        private readonly string _possibleNegativePackageStatus = "NotPresent";
        private readonly int _defaultPackageStatusRowNumber = 1;
        private readonly Dictionary<PackagesName, string> _requiredPackagesName = new()
        {
            {PackagesName.OpenSSHClient, "OpenSSH.Client~~~~0.0.1.0"},
            {PackagesName.OpenSSHServer, "OpenSSH.Server~~~~0.0.1.0"}
        };

        /// <summary>
        /// Returns the condition of all packages.
        /// </summary>
        /// <returns>Packages dictionary</returns>
        /// <exception cref="ArgumentException"></exception>
        public Dictionary<PackagesName, bool> CheckPackage()
        {
            Dictionary<PackagesName, bool> packageInstallingStatus = new();

            foreach (var packageName in _requiredPackagesName)
            {
                var result = TerminalClient.Command(_getPackagesCommand, process =>
                {
                    if (process.Error != null) throw new ArgumentException(process.Error.Message);

                    var outputValueCheck = TerminalParser.CheckValue(
                       stdOut: process.StdOut,
                       keyword: packageName.Value,
                       stateValue: _requiredPackageStatus,
                       stateRow: _defaultPackageStatusRowNumber
                   );
                    if (outputValueCheck) return true;

                    var outputCorrectnessCheck = TerminalParser.CheckValue(
                        stdOut: process.StdOut,
                        keyword: packageName.Value,
                        stateValue: _possibleNegativePackageStatus,
                        stateRow: _defaultPackageStatusRowNumber
                    );
                    if (!outputCorrectnessCheck)
                        throw new ArgumentException("Unreadable output");

                    return false;
                });

                packageInstallingStatus.Add(packageName.Key, result);
            }

            return packageInstallingStatus;
        }

        /// <summary>
        /// Installing or deleting all packages.
        /// </summary>
        /// <param name="install">
        /// If true, the packages are installed. 
        /// Otherwise, they will be removed.
        /// Default value: true
        /// </param>
        public void PackageManagment(bool install = true)
        {
            foreach (var packageName in _requiredPackagesName)
            {
                PackageManagment(packageName.Key, install);
            }
        }

        /// <summary>
        /// Selectively installing or deleting the specified packages.
        /// </summary>
        /// <param name="packageName">
        /// Accepts <see cref="PackagesName"/> which will be installed or deleted.
        /// </param>
        /// <param name="install">
        /// If true, the packages are installed. 
        /// Otherwise, they will be removed.
        /// Default value: true
        /// </param>
        /// <returns>Confirmation that the operation was successfully performed.</returns>
        public bool PackageManagment(PackagesName packageName, bool install = true)
        {
            var package = _requiredPackagesName.Single(item => item.Key == packageName);
            return TerminalClient.Command(
                PackageManagmentCommand(package.Value, install),
                CheckInstallingPackageStatus
            );
        }

        /// <summary>
        /// Returns the logical value if all the necessary packages are installed.
        /// </summary>
        /// <returns>Will return true if all the necessary packages are installed.</returns>
        internal bool CheckPackageForInitializaitonValue()
        {
            var packageDictionary = CheckPackage();
            List<bool> list = new();

            foreach (var package in packageDictionary)
                list.Add(package.Value);

            return list.TrueForAll(value => value);
        }

        /// <summary>
        /// Auxiliary method of the command construction.
        /// </summary>
        /// <returns>String type command.</returns>
        private string PackageManagmentCommand(string packageName, bool install)
        {
            string prefix = install ? "Add" : "Remove";
            return $"{prefix}-WindowsCapability -Online -Name {packageName}";
        }

        /// <summary>
        /// Auxiliary method of the checking installation or deinstallation packages.
        /// </summary>
        /// <returns>Confirmation that the operation was successfully performed.</returns>
        private bool CheckInstallingPackageStatus(TerminalCallbackOutputArgs process)
        {
            if (process.Error != null) throw process.Error;
            return TerminalParser.CheckValue(
                stdOut: process.StdOut,
                keyword: "Online",
                stateValue: "True"
            );
        }
    }
}
