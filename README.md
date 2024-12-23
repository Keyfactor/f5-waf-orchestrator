<h1 align="center" style="border-bottom: none">
    F5 WAF Universal Orchestrator Extension
</h1>

<p align="center">
  <!-- Badges -->
<img src="https://img.shields.io/badge/integration_status-production-3D1973?style=flat-square" alt="Integration Status: production" />
<a href="https://github.com/Keyfactor/f5-waf-orchestrator/releases"><img src="https://img.shields.io/github/v/release/Keyfactor/f5-waf-orchestrator?style=flat-square" alt="Release" /></a>
<img src="https://img.shields.io/github/issues/Keyfactor/f5-waf-orchestrator?style=flat-square" alt="Issues" />
<img src="https://img.shields.io/github/downloads/Keyfactor/f5-waf-orchestrator/total?style=flat-square&label=downloads&color=28B905" alt="GitHub Downloads (all assets, all releases)" />
</p>

<p align="center">
  <!-- TOC -->
  <a href="#support">
    <b>Support</b>
  </a>
  ·
  <a href="#installation">
    <b>Installation</b>
  </a>
  ·
  <a href="#license">
    <b>License</b>
  </a>
  ·
  <a href="https://github.com/orgs/Keyfactor/repositories?q=orchestrator">
    <b>Related Integrations</b>
  </a>
</p>

## Overview

The Fortigate Orchestrator Extension supports the following use cases:
1. Inventory of local user and factory cerificates
2. Ability to add new local certificates
3. Ability to renew **unbound** local user certificates
4. Ability to delete **unbound** local user certificates

The Fortigate Orchestrator Extension DOES NOT support the following use cases:
1. The renewal or removal of certificates enrolled through the internal Fortigate CA
2. The renewal or removal of factory certificates
3. The renewal or removal of ANY certificate bound to a Fortigate object
4. Certificate enrollment using the internal Fortigate CA (Keyfactor's "reenrollment" or "on device key generation" use case)

The F5 WAF Universal Orchestrator extension implements 2 Certificate Store Types. Depending on your use case, you may elect to use one, or both of these Certificate Store Types. Descriptions of each are provided below.

<details><summary>F5 WAF TLS (f5WafTls)</summary>


### f5WafTls
TODO Global Store Type Section is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info


TODO Overview is a required section
</details>

<details><summary>F5 WAF CA (f5WafCa)</summary>


### f5WafCa
TODO Global Store Type Section is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info


TODO Overview is a required section
</details>


## Compatibility

This integration is compatible with Keyfactor Universal Orchestrator version 10.4.1 and later.

## Support
The F5 WAF Universal Orchestrator extension is supported by Keyfactor for Keyfactor customers. If you have a support issue, please open a support ticket with your Keyfactor representative. If you have a support issue, please open a support ticket via the Keyfactor Support Portal at https://support.keyfactor.com. 
 
> To report a problem or suggest a new feature, use the **[Issues](../../issues)** tab. If you want to contribute actual bug fixes or proposed enhancements, use the **[Pull requests](../../pulls)** tab.

## Requirements & Prerequisites

Before installing the F5 WAF Universal Orchestrator extension, we recommend that you install [kfutil](https://github.com/Keyfactor/kfutil). Kfutil is a command-line tool that simplifies the process of creating store types, installing extensions, and instantiating certificate stores in Keyfactor Command.


The Fortigate Orchestrator Extension requires an API token be created in the Fortigate environment being managed.  Please review the following [instructions](https://docs.fortinet.com/document/forticonverter/7.0.1/online-help/866905/connect-fortigate-device-via-api-token) for creating an API token to be used in this integration.

<details><summary>F5 WAF TLS (f5WafTls)</summary>

### F5 WAF TLS Requirements
TODO Global Store Type Section is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info


TODO Requirements is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info
</details>



<details><summary>F5 WAF CA (f5WafCa)</summary>

### F5 WAF CA Requirements
TODO Global Store Type Section is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info


TODO Requirements is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info
</details>





## Create Certificate Store Types

To use the F5 WAF Universal Orchestrator extension, you **must** create the Certificate Store Types required for your usecase. This only needs to happen _once_ per Keyfactor Command instance.

The F5 WAF Universal Orchestrator extension implements 2 Certificate Store Types. Depending on your use case, you may elect to use one, or both of these Certificate Store Types.

<details><summary>F5 WAF TLS (f5WafTls)</summary>

TODO Global Store Type Section is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info


* **Create f5WafTls using kfutil**:

    ```shell
    # F5 WAF TLS
    kfutil store-types create f5WafTls
    ```

* **Create f5WafTls manually in the Command UI**:
    <details><summary>Create f5WafTls manually in the Command UI</summary>

    Create a store type called `f5WafTls` with the attributes in the tables below:

    #### Basic Tab
    | Attribute | Value | Description |
    | --------- | ----- | ----- |
    | Name | F5 WAF TLS | Display name for the store type (may be customized) |
    | Short Name | f5WafTls | Short display name for the store type |
    | Capability | f5WafTls | Store type name orchestrator will register with. Check the box to allow entry of value |
    | Supports Add | ✅ Checked | Check the box. Indicates that the Store Type supports Management Add |
    | Supports Remove | ✅ Checked | Check the box. Indicates that the Store Type supports Management Remove |
    | Supports Discovery | ✅ Checked | Check the box. Indicates that the Store Type supports Discovery |
    | Supports Reenrollment | 🔲 Unchecked |  Indicates that the Store Type supports Reenrollment |
    | Supports Create | 🔲 Unchecked |  Indicates that the Store Type supports store creation |
    | Needs Server | ✅ Checked | Determines if a target server name is required when creating store |
    | Blueprint Allowed | ✅ Checked | Determines if store type may be included in an Orchestrator blueprint |
    | Uses PowerShell | 🔲 Unchecked | Determines if underlying implementation is PowerShell |
    | Requires Store Password | 🔲 Unchecked | Enables users to optionally specify a store password when defining a Certificate Store. |
    | Supports Entry Password | 🔲 Unchecked | Determines if an individual entry within a store can have a password. |

    The Basic tab should look like this:

    ![f5WafTls Basic Tab](docsource/images/f5WafTls-basic-store-type-dialog.png)

    #### Advanced Tab
    | Attribute | Value | Description |
    | --------- | ----- | ----- |
    | Supports Custom Alias | Required | Determines if an individual entry within a store can have a custom Alias. |
    | Private Key Handling | Required | This determines if Keyfactor can send the private key associated with a certificate to the store. Required because IIS certificates without private keys would be invalid. |
    | PFX Password Style | Default | 'Default' - PFX password is randomly generated, 'Custom' - PFX password may be specified when the enrollment job is created (Requires the Allow Custom Password application setting to be enabled.) |

    The Advanced tab should look like this:

    ![f5WafTls Advanced Tab](docsource/images/f5WafTls-advanced-store-type-dialog.png)

    #### Custom Fields Tab
    Custom fields operate at the certificate store level and are used to control how the orchestrator connects to the remote target server containing the certificate store to be managed. The following custom fields should be added to the store type:

    | Name | Display Name | Description | Type | Default Value/Options | Required |
    | ---- | ------------ | ---- | --------------------- | -------- | ----------- |

    The Custom Fields tab should look like this:

    ![f5WafTls Custom Fields Tab](docsource/images/f5WafTls-custom-fields-store-type-dialog.png)



    </details>
</details>

<details><summary>F5 WAF CA (f5WafCa)</summary>

TODO Global Store Type Section is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info


* **Create f5WafCa using kfutil**:

    ```shell
    # F5 WAF CA
    kfutil store-types create f5WafCa
    ```

* **Create f5WafCa manually in the Command UI**:
    <details><summary>Create f5WafCa manually in the Command UI</summary>

    Create a store type called `f5WafCa` with the attributes in the tables below:

    #### Basic Tab
    | Attribute | Value | Description |
    | --------- | ----- | ----- |
    | Name | F5 WAF CA | Display name for the store type (may be customized) |
    | Short Name | f5WafCa | Short display name for the store type |
    | Capability | f5WafCa | Store type name orchestrator will register with. Check the box to allow entry of value |
    | Supports Add | ✅ Checked | Check the box. Indicates that the Store Type supports Management Add |
    | Supports Remove | ✅ Checked | Check the box. Indicates that the Store Type supports Management Remove |
    | Supports Discovery | ✅ Checked | Check the box. Indicates that the Store Type supports Discovery |
    | Supports Reenrollment | 🔲 Unchecked |  Indicates that the Store Type supports Reenrollment |
    | Supports Create | 🔲 Unchecked |  Indicates that the Store Type supports store creation |
    | Needs Server | ✅ Checked | Determines if a target server name is required when creating store |
    | Blueprint Allowed | ✅ Checked | Determines if store type may be included in an Orchestrator blueprint |
    | Uses PowerShell | 🔲 Unchecked | Determines if underlying implementation is PowerShell |
    | Requires Store Password | 🔲 Unchecked | Enables users to optionally specify a store password when defining a Certificate Store. |
    | Supports Entry Password | 🔲 Unchecked | Determines if an individual entry within a store can have a password. |

    The Basic tab should look like this:

    ![f5WafCa Basic Tab](docsource/images/f5WafCa-basic-store-type-dialog.png)

    #### Advanced Tab
    | Attribute | Value | Description |
    | --------- | ----- | ----- |
    | Supports Custom Alias | Required | Determines if an individual entry within a store can have a custom Alias. |
    | Private Key Handling | Forbidden | This determines if Keyfactor can send the private key associated with a certificate to the store. Required because IIS certificates without private keys would be invalid. |
    | PFX Password Style | Default | 'Default' - PFX password is randomly generated, 'Custom' - PFX password may be specified when the enrollment job is created (Requires the Allow Custom Password application setting to be enabled.) |

    The Advanced tab should look like this:

    ![f5WafCa Advanced Tab](docsource/images/f5WafCa-advanced-store-type-dialog.png)

    #### Custom Fields Tab
    Custom fields operate at the certificate store level and are used to control how the orchestrator connects to the remote target server containing the certificate store to be managed. The following custom fields should be added to the store type:

    | Name | Display Name | Description | Type | Default Value/Options | Required |
    | ---- | ------------ | ---- | --------------------- | -------- | ----------- |
    | ServerUsername | Server Username | A username (or valid PAM key if the username is stored in a KF Command configured PAM integration). If acting as an *agent* using local file access, just check *No Value* | Secret |  | 🔲 Unchecked |
    | ServerPassword | Server Password | A password (or valid PAM key if the password is stored in a KF Command configured PAM integration). The password can also be an SSH private key if connecting via SSH to a server using SSH private key authentication. If acting as an *agent* using local file access, just check *No Value* | Secret |  | 🔲 Unchecked |

    The Custom Fields tab should look like this:

    ![f5WafCa Custom Fields Tab](docsource/images/f5WafCa-custom-fields-store-type-dialog.png)



    </details>
</details>


## Installation

1. **Download the latest F5 WAF Universal Orchestrator extension from GitHub.** 

    Navigate to the [F5 WAF Universal Orchestrator extension GitHub version page](https://github.com/Keyfactor/f5-waf-orchestrator/releases/latest). Refer to the compatibility matrix below to determine whether the `net6.0` or `net8.0` asset should be downloaded. Then, click the corresponding asset to download the zip archive.
    | Universal Orchestrator Version | Latest .NET version installed on the Universal Orchestrator server | `rollForward` condition in `Orchestrator.runtimeconfig.json` | `f5-waf-orchestrator` .NET version to download |
    | --------- | ----------- | ----------- | ----------- |
    | Older than `11.0.0` | | | `net6.0` |
    | Between `11.0.0` and `11.5.1` (inclusive) | `net6.0` | | `net6.0` | 
    | Between `11.0.0` and `11.5.1` (inclusive) | `net8.0` | `Disable` | `net6.0` | 
    | Between `11.0.0` and `11.5.1` (inclusive) | `net8.0` | `LatestMajor` | `net8.0` | 
    | `11.6` _and_ newer | `net8.0` | | `net8.0` |

    Unzip the archive containing extension assemblies to a known location.

    > **Note** If you don't see an asset with a corresponding .NET version, you should always assume that it was compiled for `net6.0`.

2. **Locate the Universal Orchestrator extensions directory.**

    * **Default on Windows** - `C:\Program Files\Keyfactor\Keyfactor Orchestrator\extensions`
    * **Default on Linux** - `/opt/keyfactor/orchestrator/extensions`
    
3. **Create a new directory for the F5 WAF Universal Orchestrator extension inside the extensions directory.**
        
    Create a new directory called `f5-waf-orchestrator`.
    > The directory name does not need to match any names used elsewhere; it just has to be unique within the extensions directory.

4. **Copy the contents of the downloaded and unzipped assemblies from __step 2__ to the `f5-waf-orchestrator` directory.**

5. **Restart the Universal Orchestrator service.**

    Refer to [Starting/Restarting the Universal Orchestrator service](https://software.keyfactor.com/Core-OnPrem/Current/Content/InstallingAgents/NetCoreOrchestrator/StarttheService.htm).


6. **(optional) PAM Integration** 

    The F5 WAF Universal Orchestrator extension is compatible with all supported Keyfactor PAM extensions to resolve PAM-eligible secrets. PAM extensions running on Universal Orchestrators enable secure retrieval of secrets from a connected PAM provider.

    To configure a PAM provider, [reference the Keyfactor Integration Catalog](https://keyfactor.github.io/integrations-catalog/content/pam) to select an extension, and follow the associated instructions to install it on the Universal Orchestrator (remote).


> The above installation steps can be supplimented by the [official Command documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/InstallingAgents/NetCoreOrchestrator/CustomExtensions.htm?Highlight=extensions).



## Defining Certificate Stores

The F5 WAF Universal Orchestrator extension implements 2 Certificate Store Types, each of which implements different functionality. Refer to the individual instructions below for each Certificate Store Type that you deemed necessary for your use case from the installation section.

<details><summary>F5 WAF TLS (f5WafTls)</summary>

TODO Global Store Type Section is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info

TODO Certificate Store Configuration is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info



> The content in this section can be supplimented by the [official Command documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/ReferenceGuide/Certificate%20Stores.htm?Highlight=certificate%20store).


</details>

<details><summary>F5 WAF CA (f5WafCa)</summary>

TODO Global Store Type Section is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info

TODO Certificate Store Configuration is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info



> The content in this section can be supplimented by the [official Command documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/ReferenceGuide/Certificate%20Stores.htm?Highlight=certificate%20store).


</details>

## Discovering Certificate Stores with the Discovery Job
<details><summary>F5 WAF TLS</summary>


### F5 WAF TLS Discovery Job
TODO Global Store Type Section is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info


TODO Discovery Job Configuration is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info
</details>


<details><summary>F5 WAF CA</summary>


### F5 WAF CA Discovery Job
TODO Global Store Type Section is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info


TODO Discovery Job Configuration is an optional section. If this section doesn't seem necessary on initial glance, please delete it. Refer to the docs on [Confluence](https://keyfactor.atlassian.net/wiki/x/SAAyHg) for more info
</details>





## License

Apache License 2.0, see [LICENSE](LICENSE).

## Related Integrations

See all [Keyfactor Universal Orchestrator extensions](https://github.com/orgs/Keyfactor/repositories?q=orchestrator).