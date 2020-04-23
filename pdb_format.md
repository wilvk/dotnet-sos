from 3.1:

https://github.com/dotnet/corefx/blob/v3.1.1/src/System.Reflection.Metadata/specs/PortablePdb-Metadata.md

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

read ecma 335 spec

define a code to msil mapping file format from pdb metadata and/or a code to address mapping file format
