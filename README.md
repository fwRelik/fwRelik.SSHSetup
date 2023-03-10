# fwRelik.SSHSetup

Provides the ability to install and configure OpenSSH for Windows.

> Under the hood, the process mainly uses the [fwRelik.Terminal](https://github.com/fwRelik/fwRelik.Terminal) library.

## SSHConfigurator Class

-   Namespace: `fwRelik.SSHSetup`
-   Assembly: `fwRelik.SSHSetup.dll`

Is a general assembler of main classes. Initializes the core services at the time the [SSHConfigurator](#sshconfigurator-class) is declared.

### 📋 Methods

| Methods          | Description                                                             |
| ---------------- | ----------------------------------------------------------------------- |
| [FirewallRule]() | Returns the [FirewallRuleControl](#firewallrulecontrol-class) instance. |
| [Package]()      | Returns the [PackageControl](#packagecontrol-class) instance.           |
| [SSHService]()   | Returns the [SSHServiceControl](#sshservicecontrol-class) instance.     |
| [NetworkInfo]()  | Returns the [NetworkInfo](#networkinfo-class) instance.                 |

## FirewallRuleControl Class

-   Namespace: `fwRelik.SSHSetup.Extensions`
-   Assembly: `fwRelik.SSHSetup.dll`

### 📋 Fields

| Fields    | Description                                                |
| --------- | ---------------------------------------------------------- |
| RuleState | Provides the state of the firewall rule, default is false. |

### 📋 Methods

| Methods                | Description                        |
| ---------------------- | ---------------------------------- |
| [SetFirewallRule]()    | Sets the rule for the firewall.    |
| [RemoveFirewallRule]() | Removes the rule for the firewall. |
| [GetFirewallRule]()    | Receives the rule of the firewall. |

## PackageControl Class

-   Namespace: `fwRelik.SSHSetup.Extensions`
-   Assembly: `fwRelik.SSHSetup.dll`

### 📋 Methods

| Methods | Description |
| --- | --- |
| [CheckPackage]() | Returns the condition of all packages. |
| [PackageManagment](#packagemanagment) | Selectively installing or deleting the specified packages. `If true, the packages are installed. Otherwise, they will be removed. Default value: true` |
| [CheckPackageForInitializaitonValue]() | Returns the logical value if all the necessary packages are installed. `Will return true if all the necessary packages are installed.` |
| [GetPackagesState]() | Returns the state of the packages from the class instance. Does not in any way check for the presence of packages, it simply returns a status that is determined by actions such as installing and removing packages. To initialize the correct state, use the `CheckPackageForInitializaitonValue` or `CheckPackage` method. |

## SSHServiceControl Class

-   Namespace: `fwRelik.SSHSetup.Extensions`
-   Assembly: `fwRelik.SSHSetup.dll`

### 📋 Fields

| Fields             | Description                     |
| ------------------ | ------------------------------- |
| ServiceStatus      | Service state for this session. |
| ServiceStartupType | Display service start mode.     |

### 📋 Methods

| Methods | Description |
| --- | --- |
| [Set]() | Sets the service to the given value. Default value is `SSHServiceStartupType.Automatic`. The name of the "sshd" service, currently cannot be changed. |
| [Start]() | Starts the service. If the status of the `SSHServiceStartupType.Disabled` service will throw out the exception, since the deactivated service cannot be launched. |
| [Restart]() | Restart the service. If the status of the `SSHServiceStartupType.Disabled` service will throw out the exception, since the deactivated service cannot be launched. |
| [Stop]() | Stop the service. |
| [GetServiceStatus]() | Receives the state of the service. |

## NetworkInfo Class

-   Namespace: `fwRelik.SSHSetup.Extensions`
-   Assembly: `fwRelik.SSHSetup.dll`

### 📋 Fields

| Fields          | Description                               |
| --------------- | ----------------------------------------- |
| HostName        | Host name this device.                    |
| UserName        | The name of the current user.             |
| IpAddresses     | IP address belonging to this device.      |
| ConnectionCount | The number of connections to this device. |

### 📋 Methods

| Methods                        | Description                              |
| ------------------------------ | ---------------------------------------- |
| [GetNetworkConnectionStatus]() | Checks network connectivity.             |
| [GetConnections]()             | Receives all connections to this device. |
