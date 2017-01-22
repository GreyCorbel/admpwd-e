# LAPS.E
This projects contains LAPS.E client and support tools

Content:
- Clients:
  - Contains reference implementation of various client tools, demonstrating  how to integrate with LAPS.E.
  - Generally, integration layer for development is LAPS.E API, distrbuted in library AdmPwd.PDSWrapper.dll that allows easily consume LAPS.E service, without the need to implement any configuration on client side, implement PDS service discovery and fault tolerance - all this is implemented in the library.
  - _Note_: All client tools delivered with AdmPwd.E make use of this library as well
- KeyStores:
  - Contains source for AdmPwd.PDS.KeyStore.IKeyStore interface and implementation of PDS keystore based on Azure KeyVault
  - Provided as reference implementation for other types of key stores, such as key stores based on HSM
- LAPS.E AD schema definition file (LAPS.E.ldf), so anyone can see what changes are done in AD schema, and extend the schema manually, if needed.
