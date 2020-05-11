using System;
using System.IO;
using Microsoft.DiaSymReader.PortablePdb;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace test_pdb
{
    class Program
    {
        static void Main(string[] args)
        {
            var symBinder = new SymBinder();

            string peFilePath = Path.GetFullPath("../../artefacts/test_debug.dll");
            string pdbFilePath = Path.GetFullPath("../../artefacts/test_debug.pdb");

              //    VisualizeDebugDirectory(generation.PEReaderOpt, _writer);
              //    VisualizeGenerationIL(visualizer, generationIndex, generation, mdReader);

            var file = File.OpenRead(peFilePath);

            var peReader = new PEReader(file);

            Console.WriteLine("PER: " + peReader.PEHeaders.CoffHeader.Machine.ToString());
            var generationData = new GenerationData(peReader, peReader.GetMetadataReader(), peReader);
            Console.WriteLine("GD: " + generationData.MetadataReader.AssemblyFiles.ToString());

            Console.WriteLine("Hello World!");
        }

        private static bool IsPEStream(Stream stream)
        {
            long oldPosition = stream.Position;
            bool result = stream.ReadByte() == 'M' && stream.ReadByte() == 'Z';
            stream.Position = oldPosition;
            return result;
        }

        private static GenerationData ReadDelta(string metadataPath, string ilPathOpt)
        {
            byte[] ilDelta;
            try
            {
                ilDelta = (ilPathOpt != null) ? File.ReadAllBytes(ilPathOpt) : null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading '{ilPathOpt}': {e.Message}");
                return null;
            }

            MetadataReaderProvider mdProvider;
            try
            {
                var stream = File.OpenRead(metadataPath);

                if (!IsManagedMetadata(stream))
                {
                    throw new NotSupportedException("File format not supported");
                }

                mdProvider = MetadataReaderProvider.FromMetadataStream(stream);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading '{metadataPath}': {e.Message}");
                return null;
            }

            return new GenerationData(mdProvider, mdProvider.GetMetadataReader(), ilDelta: ilDelta);
        }

        private static bool IsManagedMetadata(Stream stream)
        {
            long oldPosition = stream.Position;
            bool result = stream.ReadByte() == 'B' && stream.ReadByte() == 'S' && stream.ReadByte() == 'J' && stream.ReadByte() == 'B';
            stream.Position = oldPosition;
            return result;
        }
  }

}
