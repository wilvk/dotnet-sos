using System;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace test_pdb
{
    public static class Metadata
    {
//        private static void VisualizeGenerationIL(MetadataVisualizer visualizer, int generationIndex, GenerationData generation, MetadataReader mdReader)
//        {
//            try
//            {
//                if (generation.PEReaderOpt != null)
//                {
//                    foreach (var methodHandle in mdReader.MethodDefinitions)
//                    {
//                        visualizer.VisualizeMethodBody(methodHandle, rva => generation.PEReaderOpt.GetMethodBody(rva));
//                    }
//                }
//                else if (generation.ILDeltaOpt != null)
//                {
//                    fixed (byte* deltaILPtr = generation.ILDeltaOpt)
//                    {
//                        foreach (var generationHandle in mdReader.MethodDefinitions)
//                        {
//                            var method = mdReader.GetMethodDefinition(generationHandle);
//                            var rva = method.RelativeVirtualAddress;
//                            if (rva != 0)
//                            {
//                                var body = MethodBodyBlock.Create(new BlobReader(deltaILPtr + rva, generation.ILDeltaOpt.Length - rva));
//
//                                visualizer.VisualizeMethodBody(body, generationHandle, generationIndex);
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    visualizer.WriteLine("<IL not available>");
//                }
//            }
//            catch (BadImageFormatException)
//            {
//                visualizer.WriteLine("<bad metadata>");
//            }
//        }

    }


}
