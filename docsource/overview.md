## Overview

The F5 WAF Universal Orchestrator extension remotely manages TLS and CA Root certificates uploaded to F5 Distributed Multi-Cloud App Connect, which is the F5 platform that manages WAF services. Certificates bound to HTTP Load Balancers within Multi-Cloud App Connect can be renewed or replaced, but they cannot be removed.

The extension uses two primary certificate store types: `f5WafTls` and `f5WafCa`. These store types are used to manage stores containing TLS and CA Root certificates, respectively. The `f5WafTls` certificate store type is focused on managing TLS certificates, which are used to enable secure communication over networks. Use cases for `f5WafTls` include discovery of TLS stores, inventorying all TLS certificates within a namespace, adding or renewing certificates, and removing unbound certificates.

On the other hand, the `f5WafCa` store type is used for managing CA Root certificates, which are essential for establishing a chain of trust between different entities. The use cases for `f5WafCa` are similar to those of `f5WafTls`, including discovery, inventory, and management of certificates. However, it is important to note that deleting a CA Root certificate replaces all instances of the same certificate within the namespace, due to an F5 WAF feature.

Overall, this extension simplifies the management of certificates within the F5 Distributed Multi-Cloud App Connect platform, providing seamless integration with Keyfactor Command.

