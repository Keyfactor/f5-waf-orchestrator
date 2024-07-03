## Overview

The `f5WafTls` certificate store type is designed to manage TLS certificates within the F5 Distributed Multi-Cloud App Connect platform. These certificates play a crucial role in enabling secure communication over networks by ensuring encrypted connections between clients and servers.

### What does it represent?
The `f5WafTls` store type represents all TLS certificates that reside within a namespace on the F5 platform. This includes both bound and unbound certificates, enabling comprehensive management and visibility into all TLS certificates in the specified namespace.

### Functionality
The `f5WafTls` certificate store type supports several key use cases, including:
1. **Discovery of TLS stores**: Identifies and returns any discoverable namespaces in the F5 WAF instance.
2. **Inventory of a TLS store**: Provides a complete inventory of all TLS certificates within a namespace, whether they are bound or unbound.
3. **Management-Add**: Allows for the addition of new certificates or the renewal of existing ones, retaining all existing bindings and applying them to the newly added or renewed certificate.
4. **Management-Delete**: Supports the removal of unbound certificates from the store. This functionality does not extend to bound certificates, which cannot be deleted.

### Caveats and Limitations
While the `f5WafTls` store type offers robust certificate management capabilities, there are some notable limitations and areas for potential confusion:
- **Bound certificates cannot be removed**: The delete operation is only applicable to unbound certificates. Bound certificates must be renewed or replaced but cannot be outright removed.
- **No SDK used**: This certificate store type does not make use of an SDK, relying instead on direct interactions with the F5 Distributed Multi-Cloud App Connect APIs.

Overall, the `f5WafTls` certificate store type is a powerful tool for managing TLS certificates within the F5 platform, offering essential functionality while also having specific limitations that users should be mindful of.

## Requirements

### Creating an F5 WAF API Token

In lieu of providing a server password when setting up an F5 WAF certificate store, F5 Multi-Cloud App Connect uses API tokens combined with the user id to authenticate when calling APIs.  API Tokens can be created through the F5 Distributed Cloud Console after logging in with the ID you wish to use for the Keyfactor certificate store.  Once logged in, select Multi-Cloud App Connect from the options under "Common services".  Next, select Account Services from the pull down at the top right of the screen, and select "Account Settings".  From there, click on "Credentials" on the left nav and "Add Credentials" on the subsequent screen.  In the form shown, select "API Token" from the Credential Type dropdown, and enter the name of the credential and the expiration date.  Please note that credentials can only be created for up to 90 day periods of time.  After 90 days, a new API token will need to be generated and replaced in your F5 WAF certificate store(s).  Clicking Generate will then show the value of the newly created API Token.  Copy this and save to a safe place, as this will be the value you will enter in the Server Password field when setting up your certificate store.  If you forget or lose this token value, there is no way to access it again in the F5 Distributed Cloud portal.  You will need to create a new API Token.

![](Images/image1.gif)
![](Images/image2.gif)
![](Images/image3.gif)
![](Images/image4.gif)
![](Images/image5.gif)
![](Images/image6.gif)

