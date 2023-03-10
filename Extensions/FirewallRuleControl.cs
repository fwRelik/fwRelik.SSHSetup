using fwRelik.Terminal;
using fwRelik.Terminal.Extensions;
using System.Text;

namespace fwRelik.SSHSetup.Extensions
{
    /// <summary>
    /// Allows you to manage the firewall rule.
    /// </summary>
    public class FirewallRuleControl
    {
        private readonly string _firewallRuleDisplayName = "OpenSSH Server (sshd)";
        private readonly string _firewallRuleName = "OpenSSH-Server-In-TCP";
        private readonly string _firewallRuleDirection = "Inbound";
        private readonly string _firewallRuleProtocol = "TCP";
        private readonly string _firewallRuleAction = "Allow";
        private readonly bool _firewallRuleEnabled = true;
        private readonly int _firewallRuleLocalPort = 22;

        /// <summary>
        /// Provides the state of the firewall rule, default is false.
        /// <remarks>
        /// Actual information will be provided 
        /// if at least one of these methods is applied: 
        /// <see cref="GetFirewallRule"/>,
        /// <see cref="SetFirewallRule"/>,
        /// <see cref="RemoveFirewallRule"/>.
        /// </remarks>
        /// </summary>
        public bool RuleState { get; private set; } = false;

        /// <summary>
        /// Removes the rule for the firewall.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void RemoveFirewallRule()
        {
            TerminalClient.Command(RemoveFirewallRuleCommand(), process =>
            {
                if (process.Error != null)
                {
                    RuleState = true;
                    throw new ArgumentException(process.Error.Message);
                }
                RuleState = false;
            });
        }

        /// <summary>
        /// Sets the rule for the firewall.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void SetFirewallRule()
        {
            TerminalClient.Command(SetFirewallRuleCommand(), process =>
            {
                if (process.Error != null)
                {
                    RuleState = false;
                    throw new ArgumentException(process.Error.Message);
                }
                RuleState = true;
            });
        }

        /// <summary>
        /// Receives the rule of the firewall.
        /// </summary>
        /// <returns>Returns the tuple</returns>
        /// <exception cref="ArgumentException"></exception>
        public (bool FirewallRuleStatus, string Description) GetFirewallRule()
        {
            bool firewallRule = TerminalClient.Command(GetFirewallRuleCommand(), process =>
            {
                if (process.Error != null) throw new ArgumentException(process.Error.Message);
                return TerminalParser.CheckValue(
                    stdOut: process.StdOut,
                    keyword: "True",
                    stateValue: "True"
                );
            });

            RuleState = firewallRule;
            string description = firewallRule
                ? $"Firewall Rule '{_firewallRuleDisplayName}' has been exists."
                : $"Firewall Rule '{_firewallRuleDisplayName}' does not exist.";

            return (firewallRule, description);
        }

        /// <summary>
        /// Remove firewall rule command builder.
        /// </summary>
        /// <returns>Remove firewall rule command.</returns>
        private string RemoveFirewallRuleCommand()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append("Remove-NetFirewallRule ")
                .Append($"-Name '{_firewallRuleName}'");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Set firewall rule command builder.
        /// </summary>
        /// <returns>Set firewall rule command.</returns>
        private string SetFirewallRuleCommand()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append("New-NetFirewallRule ")
                .Append($"-Name '{_firewallRuleName}' ")
                .Append($"-DisplayName '{_firewallRuleDisplayName}' ")
                .Append($"-Enabled {_firewallRuleEnabled} ")
                .Append($"-Direction {_firewallRuleDirection} ")
                .Append($"-Protocol {_firewallRuleProtocol} ")
                .Append($"-Action {_firewallRuleAction} ")
                .Append($"-LocalPort {_firewallRuleLocalPort} ");

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Get firewall rule command builder.
        /// </summary>
        /// <returns>Get firewall rule command.</returns>
        private string GetFirewallRuleCommand()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append("!!(Get-NetFirewallRule ")
                .Append($"-Name '{_firewallRuleName}' ")
                .Append("-ErrorAction SilentlyContinue | Select-Object Name, Enabled)");

            return stringBuilder.ToString();
        }
    }
}
