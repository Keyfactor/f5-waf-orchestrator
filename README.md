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

The F5 WAF Universal Orchestrator extension remotely manages TLS and CA Root certificates uploaded to F5 Distributed Multi-Cloud App Connect, which is the F5 platform that manages WAF services. Certificates bound to HTTP Load Balancers within Multi-Cloud App Connect can be renewed or replaced, but they cannot be removed.

The extension uses two primary certificate store types: `f5WafTls` and `f5WafCa`. These store types are used to manage stores containing TLS and CA Root certificates, respectively. The `f5WafTls` certificate store type is focused on managing TLS certificates, which are used to enable secure communication over networks. Use cases for `f5WafTls` include discovery of TLS stores, inventorying all TLS certificates within a namespace, adding or renewing certificates, and removing unbound certificates.

On the other hand, the `f5WafCa` store type is used for managing CA Root certificates, which are essential for establishing a chain of trust between different entities. The use cases for `f5WafCa` are similar to those of `f5WafTls`, including discovery, inventory, and management of certificates. However, it is important to note that deleting a CA Root certificate replaces all instances of the same certificate within the namespace, due to an F5 WAF feature.

Overall, this extension simplifies the management of certificates within the F5 Distributed Multi-Cloud App Connect platform, providing seamless integration with Keyfactor Command.

## Compatibility

This integration is compatible with Keyfactor Universal Orchestrator version 10.4.1 and later.

## Support
The F5 WAF Universal Orchestrator extension is supported by Keyfactor for Keyfactor customers. If you have a support issue, please open a support ticket with your Keyfactor representative. If you have a support issue, please open a support ticket via the Keyfactor Support Portal at https://support.keyfactor.com. 
 
> To report a problem or suggest a new feature, use the **[Issues](../../issues)** tab. If you want to contribute actual bug fixes or proposed enhancements, use the **[Pull requests](../../pulls)** tab.

## Installation
Before installing the F5 WAF Universal Orchestrator extension, it's recommended to install [kfutil](https://github.com/Keyfactor/kfutil). Kfutil is a command-line tool that simplifies the process of creating store types, installing extensions, and instantiating certificate stores in Keyfactor Command.

The F5 WAF Universal Orchestrator extension implements 2 Certificate Store Types. Depending on your use case, you may elect to install one, or all of these Certificate Store Types. An overview for each type is linked below:
* [F5 WAF TLS](docs/f5waftls.md)
* [F5 WAF CA](docs/f5wafca.md)

<details><summary>F5 WAF TLS</summary>


1. Follow the [requirements section](docs/f5waftls.md#requirements) to configure a Service Account and grant necessary API permissions.

    <details><summary>Requirements</summary>

    ### Creating an F5 WAF API Token

    In lieu of providing a server password when setting up an F5 WAF certificate store, F5 Multi-Cloud App Connect uses API tokens combined with the user id to authenticate when calling APIs.  API Tokens can be created through the F5 Distributed Cloud Console after logging in with the ID you wish to use for the Keyfactor certificate store.  Once logged in, select Multi-Cloud App Connect from the options under "Common services".  Next, select Account Services from the pull down at the top right of the screen, and select "Account Settings".  From there, click on "Credentials" on the left nav and "Add Credentials" on the subsequent screen.  In the form shown, select "API Token" from the Credential Type dropdown, and enter the name of the credential and the expiration date.  Please note that credentials can only be created for up to 90 day periods of time.  After 90 days, a new API token will need to be generated and replaced in your F5 WAF certificate store(s).  Clicking Generate will then show the value of the newly created API Token.  Copy this and save to a safe place, as this will be the value you will enter in the Server Password field when setting up your certificate store.  If you forget or lose this token value, there is no way to access it again in the F5 Distributed Cloud portal.  You will need to create a new API Token.

    ![](Images/image1.gif)
    ![](Images/image2.gif)
    ![](Images/image3.gif)
    ![](Images/image4.gif)
    ![](Images/image5.gif)
    ![](Images/image6.gif)



    </details>

2. Create Certificate Store Types for the F5 WAF Orchestrator extension. 

    * **Using kfutil**:

        ```shell
        # F5 WAF TLS
        kfutil store-types create f5WafTls
        ```

    * **Manually**:
        * [F5 WAF TLS](docs/f5waftls.md#certificate-store-type-configuration)

3. Install the F5 WAF Universal Orchestrator extension.
    
    * **Using kfutil**: On the server that that hosts the Universal Orchestrator, run the following command:

        ```shell
        # Windows Server
        kfutil orchestrator extension -e f5-waf-orchestrator@latest --out "C:\Program Files\Keyfactor\Keyfactor Orchestrator\extensions"

        # Linux
        kfutil orchestrator extension -e f5-waf-orchestrator@latest --out "/opt/keyfactor/orchestrator/extensions"
        ```

    * **Manually**: Follow the [official Command documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/InstallingAgents/NetCoreOrchestrator/CustomExtensions.htm?Highlight=extensions) to install the latest [F5 WAF Universal Orchestrator extension](https://github.com/Keyfactor/f5-waf-orchestrator/releases/latest).

4. Create new certificate stores in Keyfactor Command for the Sample Universal Orchestrator extension.

    * [F5 WAF TLS](docs/f5waftls.md#certificate-store-configuration)


</details>

<details><summary>F5 WAF CA</summary>


1. Follow the [requirements section](docs/f5wafca.md#requirements) to configure a Service Account and grant necessary API permissions.

    <details><summary>Requirements</summary>

    ### Creating an F5 WAF API Token

    In lieu of providing a server password when setting up an F5 WAF certificate store, F5 Multi-Cloud App Connect uses API tokens combined with the user id to authenticate when calling APIs.  API Tokens can be created through the F5 Distributed Cloud Console after logging in with the ID you wish to use for the Keyfactor certificate store.  Once logged in, select Multi-Cloud App Connect from the options under "Common services".  Next, select Account Services from the pull down at the top right of the screen, and select "Account Settings".  From there, click on "Credentials" on the left nav and "Add Credentials" on the subsequent screen.  In the form shown, select "API Token" from the Credential Type dropdown, and enter the name of the credential and the expiration date.  Please note that credentials can only be created for up to 90 day periods of time.  After 90 days, a new API token will need to be generated and replaced in your F5 WAF certificate store(s).  Clicking Generate will then show the value of the newly created API Token.  Copy this and save to a safe place, as this will be the value you will enter in the Server Password field when setting up your certificate store.  If you forget or lose this token value, there is no way to access it again in the F5 Distributed Cloud portal.  You will need to create a new API Token.

    ![](Images/image1.gif)
    ![](Images/image2.gif)
    ![](Images/image3.gif)
    ![](Images/image4.gif)
    ![](Images/image5.gif)
    ![](Images/image6.gif)



    </details>

2. Create Certificate Store Types for the F5 WAF Orchestrator extension. 

    * **Using kfutil**:

        ```shell
        # F5 WAF CA
        kfutil store-types create f5WafCa
        ```

    * **Manually**:
        * [F5 WAF CA](docs/f5wafca.md#certificate-store-type-configuration)

3. Install the F5 WAF Universal Orchestrator extension.
    
    * **Using kfutil**: On the server that that hosts the Universal Orchestrator, run the following command:

        ```shell
        # Windows Server
        kfutil orchestrator extension -e f5-waf-orchestrator@latest --out "C:\Program Files\Keyfactor\Keyfactor Orchestrator\extensions"

        # Linux
        kfutil orchestrator extension -e f5-waf-orchestrator@latest --out "/opt/keyfactor/orchestrator/extensions"
        ```

    * **Manually**: Follow the [official Command documentation](https://software.keyfactor.com/Core-OnPrem/Current/Content/InstallingAgents/NetCoreOrchestrator/CustomExtensions.htm?Highlight=extensions) to install the latest [F5 WAF Universal Orchestrator extension](https://github.com/Keyfactor/f5-waf-orchestrator/releases/latest).

4. Create new certificate stores in Keyfactor Command for the Sample Universal Orchestrator extension.

    * [F5 WAF CA](docs/f5wafca.md#certificate-store-configuration)


</details>


## License

Apache License 2.0, see [LICENSE](LICENSE).

## Related Integrations

See all [Keyfactor Universal Orchestrator extensions](https://github.com/orgs/Keyfactor/repositories?q=orchestrator).