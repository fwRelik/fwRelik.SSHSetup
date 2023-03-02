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
        private readonly Dictionary<PackagesName, bool> _requiredPackagesInstallingStatus = new()
        {
            {PackagesName.OpenSSHClient, false},
            {PackagesName.OpenSSHServer, false}
        };

        /// <summary>
        /// Returns the condition of all packages.
        /// </summary>
        /// <returns>Packages dictionary</returns>
        /// <exception cref="ArgumentException"></exception>
        public Dictionary<PackagesName, bool> CheckPackage()
        {
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

                _requiredPackagesInstallingStatus[packageName.Key] = result;
            }

            return _requiredPackagesInstallingStatus;
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
            bool installingResult = TerminalClient.Command(
                PackageManagmentCommand(package.Value, install),
                CheckInstallingPackageStatus
            );

            _requiredPackagesInstallingStatus[packageName] = installingResult;
            return installingResult;
        }

        /// <summary>
        /// Returns the logical value if all the necessary packages are installed.
        /// </summary>
        /// <returns>Will return true if all the necessary packages are installed.</returns>
        public bool CheckPackageForInitializaitonValue()
        {
            var packageDictionary = CheckPackage();
            return packageDictionary.Values.ToList().TrueForAll(v => v);
        }

        /// <summary>
        /// Returns the state of the packages from the class instance.
        /// </summary>
        /// <remarks>
        /// Does not in any way check for the presence of packages, 
        /// it simply returns a status that is determined by actions 
        /// such as installing and removing packages.
        /// To initialize the correct state, 
        /// use the <see cref="CheckPackageForInitializaitonValue"/> or <see cref="CheckPackage"/> method.
        /// </remarks>
        /// <returns>Returns a tuple with the general state and the dictionary itself with package names.</returns>
        public (bool AllPackageInstalling, Dictionary<PackagesName, bool> PackagesDictionary) GetPackagesState()
        {
            return (
                _requiredPackagesInstallingStatus.Values.ToList().TrueForAll(v => v),
                _requiredPackagesInstallingStatus
            );
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
