# f5-waf-orchestrator

The F5 WAF Orchestrator is an extension to the Keyfactor Universal Orchestrator. It Integrates with the F5 Distributed Cloud Multi-App Connect service to facilitate the inventory, addition, removal, and discovery of certificates for WAF applications. 

#### Integration status: Prototype

## About the Keyfactor Universal Orchestrator Extension

This repository contains a Universal Orchestrator Extension which is a plugin to the Keyfactor Universal Orchestrator. Within the Keyfactor Platform, Orchestrators are used to manage “certificate stores” &mdash; collections of certificates and roots of trust that are found within and used by various applications.

The Universal Orchestrator is part of the Keyfactor software distribution and is available via the Keyfactor customer portal. For general instructions on installing Extensions, see the “Keyfactor Command Orchestrator Installation and Configuration Guide” section of the Keyfactor documentation. For configuration details of this specific Extension see below in this readme.

The Universal Orchestrator is the successor to the Windows Orchestrator. This Orchestrator Extension plugin only works with the Universal Orchestrator and does not work with the Windows Orchestrator.

## Support for F5 WAF Orchestrator

The F5 WAF Orchestrator is open source and supported on best effort level for this tool/library/client.  This means customers can report Bugs, Feature Requests, Documentation amendment or questions as well as requests for customer information required for setup that needs Keyfactor access to obtain. Such requests do not follow normal SLA commitments for response or resolution. If you have a support issue, please open a support ticket via the Keyfactor Support Portal at https://support.keyfactor.com/.

###### To report a problem or suggest a new feature, use the **[Issues](../../issues)** tab. If you want to contribute actual bug fixes or proposed enhancements, use the **[Pull requests](../../pulls)** tab.

---


---

## Keyfactor Version Supported

The minimum version of the Keyfactor Universal Orchestrator Framework needed to run this version of the extension is 10.2.

## Platform Specific Notes

The Keyfactor Universal Orchestrator may be installed on either Windows or Linux based platforms. The certificate operations supported by a capability may vary based what platform the capability is installed on. The table below indicates what capabilities are supported based on which platform the encompassing Universal Orchestrator is running.
| Operation | Win | Linux |
|-----|-----|------|
|Supports Management Add|&check; |&check; |
|Supports Management Remove|&check; |&check; |
|Supports Create Store|  |  |
|Supports Discovery|&check; |&check; |
|Supports Renrollment|  |  |
|Supports Inventory|&check; |&check; |

---

## Overview
The F5 WAF Orchestrator extension remotely manages certificates used by F5 Distributed Cloud WAF.

## Overview
The F5 WAF Orchestrator extension remotely manages certificates uploaded to F5 Distributed Cloud Multi-App Connect, which is the F5 platform that manages WAF services. Once in Multi-App Connect, certificates can be associated to configured HTTP load balancers. 

## F5 WAF Orchestrator Installation

1. Stop the Keyfactor Universal Orchestrator Service.
2. In the Keyfactor Orchestrator installation folder (by convention usually C:\Program Files\Keyfactor\Keyfactor Orchestrator), find the "extensions" folder. Underneath that, create a new folder named F5 or another name of your choosing.
3. Download the latest version of the F5 Orchestrator from GitHub.
4. Copy the contents of the download installation zip file into the folder created in step 1.
5. Start the Keyfactor Universal Orchestrator Service. 

## F5 WAF Orchestrator Configuration

Below are the steps for manually configuring the F5 WAF Orchestrator in Keyfactor Command, assuming the Keyfactor Universal Orchestrator is installed and has the F5 WAF Orchestrator extension: 

1. Create a Certificate Store Type for the F5 WAF Orchestrator extension.
2. Create a new certificate store in Keyfactor Command for the F5 WAF Orchestrator extension.

View the Certificate Store Type and certificate store configuration instructions for the F5 WAF Orchestrator below.

## Certificate Store Type Configuration

The `F5Cloud` Certificate Store Type can be created manually by following the below steps:

Create a store type called `F5Cloud` with the attributes in the tables below:

### Basic Tab
| Attribute | Value | Description |
| --------- | ----- | ----- |
| Name | Azure Application (Auth) | Display name for the store type (may be customized) |
| Short Name | AzureApp | Short display name for the store type |
| Capability | AzureApp | Store type name orchestrator will register with. Check the box to allow entry of value |
| Supported Job Types (check the box for each) | Add, Discovery, Remove | Job types the extension supports |
| Needs Server | &check; | Determines if a target server name is required when creating store |
| Blueprint Allowed |  | Determines if store type may be included in an Orchestrator blueprint |
| Uses PowerShell |  | Determines if underlying implementation is PowerShell |
| Requires Store Password |  | Determines if a store password is required when configuring an individual store. |
| Supports Entry Password |  | Determines if an individual entry within a store can have a password. |


The Basic tab should look like this:

![AzureApp Basic Tab](../.github/images/AzureApp-basic-store-type-dialog.png)

### Advanced Tab
| Attribute | Value | Description |
| --------- | ----- | ----- |
| Supports Custom Alias | Required | Determines if an individual entry within a store can have a custom Alias. |
| Private Key Handling | Forbidden | This determines if Keyfactor can send the private key associated with a certificate to the store. Required because IIS certificates without private keys would be invalid. |
| PFX Password Style | Default | 'Default' - PFX password is randomly generated, 'Custom' - PFX password may be specified when the enrollment job is created (Requires the Allow Custom Password application setting to be enabled.) |


The Advanced tab should look like this:

![AzureApp Advanced Tab](../.github/images/AzureApp-advanced-store-type-dialog.png)

### Custom Fields Tab
Custom fields operate at the certificate store level and are used to control how the orchestrator connects to the remote target server containing the certificate store to be managed. The following custom fields should be added to the store type:

| Name | Display Name | Type | Default Value/Options | Required | Description |
| ---- | ------------ | ---- | --------------------- | -------- | ----------- |
| ServerUsername | Server Username | Secret |  |  | The Application ID of the Service Principal used to authenticate with Microsoft Graph for managing Application/Service Principal certificates. |
| ServerPassword | Server Password | Secret |  |  | A Client Secret that the extension will use to authenticate with Microsoft Graph for managing Application/Service Principal certificates. |
| ServerUseSsl | Use SSL | Bool | true | &check; | Specifies whether SSL should be used for communication with the server. Set to 'true' to enable SSL, and 'false' to disable it. |


The Custom Fields tab should look like this:

![AzureApp Custom Fields Tab](../.github/images/AzureApp-custom-fields-store-type-dialog.png)

## Certificate Store Configuration

After creating the `F5Cloud` Certificate Store Type and installing the Azure App Registration and Enterprise Application Orchestrator extension, you can create new [Certificate Stores](https://software.keyfactor.com/Core-OnPrem/Current/Content/ReferenceGuide/Certificate%20Stores.htm?Highlight=certificate%20store) to manage certificates in the remote platform.

The following table describes the required and optional fields for the `F5Cloud` certificate store type.

| Attribute | Description |
| --------- | ----------- |
| Category | Select Azure Application (Auth)  or the customized certificate store name from the previous step. |
| Container | Optional container to associate certificate store with. |
| Client Machine | The Azure Tenant (directory) ID that owns the Service Principal. |
| Store Path | The Application ID of the target Application/Service Principal that will be managed by the Azure App Registration and Enterprise Application Orchestrator extension. |
| Orchestrator | Select an approved orchestrator capable of managing AzureApp certificates. Specifically, one with the AzureApp capability. |
| Server Username | The Application ID of the Service Principal used to authenticate with Microsoft Graph for managing Application/Service Principal certificates. |
| Server Password | A Client Secret that the extension will use to authenticate with Microsoft Graph for managing Application/Service Principal certificates. |
| Use SSL | Specifies whether SSL should be used for communication with the server. Set to 'true' to enable SSL, and 'false' to disable it. |

* In Keyfactor Command, navigate to Certificate Stores from the Locations Menu. Click the Add button to create a new Certificate Store using the attributes in the table above.
--



