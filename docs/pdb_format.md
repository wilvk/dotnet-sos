from 3.1:

pdb overview:
https://github.com/dotnet/corefx/blob/v3.1.1/src/System.Reflection.Metadata/specs/PortablePdb-Metadata.md

assembly manifest:
https://docs.microsoft.com/en-us/dotnet/standard/assembly/manifest


repreesentations:

source code -> binary + pdb

               binary -> msil -> runtime -> jit compiler -> assembly

               pdb -> metadata -> code to msil mapping


                         msil -> assembly manifest, type metadata


mapping:

  source code <-> pdb (assembly metadata) <-> msil <-> assembly


msil cil doc:
https://www.c-sharpcorner.com/UploadFile/ajyadav123/msil-programming-part-1/

ECMA 335 specification:

want to know:

for setting a breakpoint in a file at a line:
- what is the method the line is in?
- what is the line of the method?
- What is the corresponding method and line in the metadata?
- What is the corresponding reference in the msil for the line and metadata?
- What is the corresponding assembly address to break on for the msil reference?

todo:

- [ ] read ecma 335 spec
- [ ] define a code to msil mapping file format from pdb metadata and/or a code to address mapping file format
- [ ] build and run ildasm for dotnet core


## Format

sequence-points
tables:
  Document
  MethodDebugInformation

## ECMA 335

Partition II - Metadata Definitions and Semantics
Partition V - Debug Interchange Format - CLI producres and consumers


p32:

metadata: Data that describes and references the types defined by the CTS. Metadata is stored in a way that is independent of any particular programming language. Thus, metadata provides a common interchange mechanism for use between tools that manipulate programs (such as compilers and debuggers) as well as between these tools and the VES.

p36:
Relationship to managed metadata-driven execution


-
