## Overview

The `f5WafCa` certificate store type is designed to manage CA Root certificates within the F5 Distributed Multi-Cloud App Connect platform. CA Root certificates are critical components that establish a chain of trust between entities, ensuring the authenticity and reliability of certificates issued by intermediate Certificate Authorities.

### What does it represent?
The `f5WafCa` store type represents all CA Root certificates residing within a namespace on the F5 platform. This comprehensive management scope includes discovery, inventory, addition, renewal, and deletion of CA Root certificates within the specified namespace.

### Functionality
The `f5WafCa` certificate store type supports several key use cases, including:
1. **Discovery of CA Root stores**: Identifies and returns any discoverable namespaces in the F5 WAF instance.
2. **Inventory of a CA Root store**: Provides a complete inventory of all CA Root certificates within a namespace.
3. **Management-Add**: Allows the addition of new certificates or the renewal of existing ones.
4. **Management-Delete**: Supports the removal of existing certificates. However, deleting a CA Root certificate replaces all instances of the same certificate within the namespace, which is an intrinsic F5 WAF feature beyond the control of this integration.

### Caveats and Limitations
While the `f5WafCa` store type offers essential capabilities for managing CA Root certificates, there are some notable limitations and areas for potential confusion:
- **Replacing Multiple Instances**: Deleting a CA Root certificate will replace every instance of that certificate across the namespace, not just the one represented by the intended alias. Users must be cautious and aware of this behavior.
- **No SDK used**: This certificate store type does not utilize an SDK, relying instead on direct interactions with the F5 Distributed Multi-Cloud App Connect APIs.

Overall, the `f5WafCa` certificate store type is a powerful tool for managing CA Root certificates within the F5 platform, providing extensive functionality while also having specific limitations that users need to be aware of.

## Requirements

### Creating an F5 WAF API Token

In lieu of providing a server password when setting up an F5 WAF certificate store, F5 Multi-Cloud App Connect uses API tokens combined with the user id to authenticate when calling APIs.  API Tokens can be created through the F5 Distributed Cloud Console after logging in with the ID you wish to use for the Keyfactor certificate store.  Once logged in, select Multi-Cloud App Connect from the options under "Common services".  Next, select Account Services from the pull down at the top right of the screen, and select "Account Settings".  From there, click on "Credentials" on the left nav and "Add Credentials" on the subsequent screen.  In the form shown, select "API Token" from the Credential Type dropdown, and enter the name of the credential and the expiration date.  Please note that credentials can only be created for up to 90 day periods of time.  After 90 days, a new API token will need to be generated and replaced in your F5 WAF certificate store(s).  Clicking Generate will then show the value of the newly created API Token.  Copy this and save to a safe place, as this will be the value you will enter in the Server Password field when setting up your certificate store.  If you forget or lose this token value, there is no way to access it again in the F5 Distributed Cloud portal.  You will need to create a new API Token.

![](Images/image1.gif)
![](Images/image2.gif)
![](Images/image3.gif)
![](Images/image4.gif)
![](Images/image5.gif)
![](Images/image6.gif)

