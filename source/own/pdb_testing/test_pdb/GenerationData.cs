using System;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace test_pdb
{

  public class GenerationData
  {
      public readonly IDisposable MemoryOwner;
      public readonly MetadataReader MetadataReader;
      public readonly PEReader PEReaderOpt;
      public readonly byte[] ILDeltaOpt;

      public GenerationData(IDisposable memoryOwner, MetadataReader metadataReader, PEReader peReader = null, byte[] ilDelta = null)
      {
          MemoryOwner = memoryOwner;
          MetadataReader = metadataReader;
          PEReaderOpt = peReader;
          ILDeltaOpt = ilDelta;
      }
  }
}
