using fwRelik.SSHSetup.Extensions;

namespace fwRelik.SSHSetup
{
    /// <summary>
    /// Initializes the core services at the time the <see cref="SSHConfigurator"/> is declared.
    /// </summary>
    public class SSHConfigurator
    {
        private readonly SSHServiceControl _sshServiceControl = new();
        private readonly FirewallRuleControl _firewallRuleControl = new();
        private readonly PackageControl _packageControl = new();
        private readonly NetworkInfo _networkInfo = new();

        /// <summary>
        /// Returns the <see cref="SSHServiceControl"/> instance.
        /// </summary>
        /// <returns><see cref="SSHServiceControl"/></returns>
        public SSHServiceControl SSHService() => _sshServiceControl;

        /// <summary>
        /// Returns the <see cref="FirewallRuleControl"/> instance.
        /// </summary>
        /// <returns><see cref="FirewallRuleControl"/></returns>
        public FirewallRuleControl FirewallRule() => _firewallRuleControl;

        /// <summary>
        /// Returns the <see cref="PackageControl"/> instance.
        /// </summary>
        /// <returns><see cref="PackageControl"/></returns>
        public PackageControl Package() => _packageControl;

        /// <summary>
        /// Returns the <see cref="NetworkInfo"/> instance.
        /// </summary>
        /// <returns><see cref="NetworkInfo"/></returns>
        public NetworkInfo NetworkInfo() => _networkInfo;
    }
}