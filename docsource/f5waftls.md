## Overview

The f5WafTls certificate store type is used to manage F5 Distributed Multi-Cloud App Connect TLS certificates.

Use cases supported:
1. Discovery of TLS stores.  Discovery for F5 WAF returns any discoverable namespaces in the F5 WAF instance.
2. Inventory of a TLS store.  All TLS certificates, bound or unbound, within a namespace will be returned to Keyfactor Command.
3. Management-Add.  Add a new certificate or renew an existing one.  Renew will work for both bound and unbound certificates.  All existing binding will remain in place, bound to the same alias with the newly replaced/renewed certificate.
4. Management-Delete.  Remove an existing certificate.  Will only work for unbound certificates.


