## Overview

The F5 WAF Orchestrator extension remotely manages TLS and CA Root certificates uploaded to F5 Distributed Multi-Cloud App Connect, which is the F5 platform that manages WAF services. Certificates bound to Http Load Balancers within Multi-Cloud App Connect can be renewed/replaced, but they cannot be removed. Certificate store types f5WafTls and f5WafCa are used to manage stores containing TLS and CA Root certificates, respectively.


## Requirements

F5 Multi-Cloud App Connect uses API tokens to authenticate when calling APIs.  API Tokens can be created through the F5 Distributed Cloud Console.  Once logged in, select Multi-Cloud App Connect from the options under "Common services".  Next, select Account Services from the pull down at the top right of the screen, and select "Account Settings".  From there, click on "Credentials" on the left nav and "Add Credentials" on the subsequent screen.  In the form shown, select "API Token" from the Credential Type dropdown, and enter the name of the credential and the expiration date.  Please note that credentials can only be created for up to 90 day periods of time.  After 90 days, a new API token will need to be generated and replaced in your F5 WAF certificate store(s).  Clicking Generate will then show the value of the newly created API Token.  Copy this and save to a safe place, as this will be the value you will enter in the Server Password field when setting up your certificate store.  If you forget or lose this token value, there is no way to access it again in the F5 Distributed Cloud portal.  You will need to create a new API Token.

![](Images/image1.gif)
![](Images/image2.gif)
![](Images/image3.gif)
![](Images/image4.gif)
![](Images/image5.gif)
![](Images/image6.gif)


## Discovery

The following table describes the required and optional fields to schedule a Discovery job for the `f5WafTls` and `f5WafCa` certificate store types.

In Keyfactor Command, navigate to Certificate Stores from the Locations Menu and then click on the Discover tab.

| Attribute | Description                                                                                                                                    |
| --------- |------------------------------------------------------------------------------------------------------------------------------------------------|
| Category | Select either F5WafTls or F5WafCa depending on whether you want to return namespaces for TLS certificates or CA Root certificates.                                        |
| Orchestrator | Select an approved orchestrator capable of managing F5 WAF certificates. Specifically, one with the f5WafTls and f5WafCa capabilities.         |
| Schedule | Enter the schedule for when you want the job to run   |
| Client Machine | The URL for the F5 Distributed Cloud instance (typically ending in '.console.ves.volterra.io'.                                                 |
| Server Username | This is not used but required in the UI.  Enter any value. |
| Server Password | The API Token configured in the F5 Distributed Cloud instance's Account Settings.  Please see [Requirements](#requirements) for more details on creating this token.  |
| Directories to Search | Not used for this integration.  Leave Blank.  |
| Directories to ignore | Not used for this integration.  Leave Blank.  |
| Extensions | Not used for this integration.  Leave Blank.  |
| File name patterns to match | Not used for this integration.  Leave Blank.  |
| Follow SymLinks | Not used for this integration.  Leave Unchecked.  |  
| Follow SymLinks | Not used for this integration.  Leave Unchecked.  |  
| Use SSL? | Not used for this integration.  Leave Unchecked.  |  

Discovery jobs will return all known namespaces for this F5 WAF instance.  Please note that because Keyfactor Command has a restriction on multiple certificate stores having the same Client Machine and Store Path, certificate stores for f5WafTls will return stores with a "tls-" prefixed to the beginning of the store path (namespace); while f5WafCA stores will have "ca-" prefixed.  Any jobs that run for stores with these prefixes will have these prefixes removed before calling any F5 WAF APIs.  What this means is a store path (namespace) for an f5WafTls store of "tls-namespace1" will be the same as one labeled "namespace1".