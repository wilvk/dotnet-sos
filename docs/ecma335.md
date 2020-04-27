# Partition II

p140:
An assembly is a set of one or more files deployed as a unit. An assembly always contains a manifest that specifies:

- Version, name, culture, and security requirements for the assembly.

- Which other files, if any, belong to the assembly, along with a cryptographic hash of each file. The manifest itself resides in the metadata part of a file, and that file is always part of the assembly.

- The types defined in other files of the assembly that are to be exported from the assembly. Types defined in the same file as the manifest are exported based on attributes of the type itself.

- Optionally, a digital signature for the manifest itself, and the public key used to compute it.

A module is a single file containing executable content in the format specified here. If the module contains a manifest then it also specifies the modules (including itself) that constitute the assembly. An assembly shall contain only one manifest amongst all its constituent files. For an assembly that is to be executed (rather than simply being dynamically loaded) the manifest shall reside in the module that contains the entry point.
