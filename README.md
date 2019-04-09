# AdmPwd.E
This repository contains AdmPwd.E (formerly known as LAPS.E) sample client and support tools and contains of the following projects as of now:

- Clients
- Keystores
- LAPS Cleanup tools

More on projects:
- Clients:
  - Contains reference implementation of various client tools, demonstrating  how to integrate with AdmPwd.E using PDS Wrapper integration library
  - Generally, integration layer for development of integrations is AdmPwd.E API, distrbuted in library AdmPwd.PDSWrapper.dll that allows easily consume AdmPwd.E service, without the need to implement any configuration on client side, implement PDS service discovery and fault tolerance - all this is implemented in the library.
  - _Note_: All client tools delivered with AdmPwd.E make use of this library as well
- KeyStores:
  - Contains source for AdmPwd.PDS.KeyStore.IKeyStore interface. This is interface required to implement when you want PDS to use it for key storage
  - Contains reference implementation of PDS keystore that stores keys in Azure KeyVault
  - Provided as reference implementation for other types of key stores, such as key stores based on HSM
- LAPS Cleanup: 
  - various Powershell scripts that help review and cleanup LAPS specific data in AD (e.g. permissions setup by delegation cmdlets), which may come useful when moving from LAPS to AdmPwd.E

Repository also contains AdmPwd.E AD schema definition file (AdmPwd.E.ldf), so anyone can see what changes are required in AD schema for AdmPwd.E, and extend the schema manually, if needed.

