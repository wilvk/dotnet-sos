using System;
using System.IO;
using Microsoft.DiaSymReader;
using Microsoft.DiaSymReader.PortablePdb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;
using Roslyn.Utilities;

namespace test_pdb
{
    class Program
    {
        static void Main(string[] args)
        {
            var symBinder = new SymBinder();

            string peFilePath = Path.GetFullPath("../../artefacts/test_debug.dll");
            string pdbFilePath = Path.GetFullPath("../../artefacts/test_debug.pdb");

            //MemoryStream ms = new MemoryStream();
            //FileStream file = new FileStream(pdbFilePath, FileMode.Open, FileAccess.Read);
            //file.CopyTo(ms);

            var file = File.OpenRead(peFilePath);

            var peReader = new PEReader(file);

            Console.WriteLine("PER: " + peReader.PEHeaders.CoffHeader.Machine.ToString());
            var generationData = new GenerationData(peReader, peReader.GetMetadataReader(), peReader);
            Console.WriteLine("GD: " + generationData.MetadataReader.AssemblyFiles.ToString());

            Console.WriteLine("Hello World!");
        }

        private int RunOne(string peFilePath)
        {
            var generations = new List<GenerationData>();
            var embeddedPdb = false;

            ///g:<metadata-delta-path>;<il-delta-path>  Add generation delta blobs.
            var generationDeltas = new List<(string, string)>();

            var generation = ReadBaseline(peFilePath, embeddedPdb);

            if (generation == null)
            {
                return 1;
            }

            generations.Add(generation);

            int i = 1;
            foreach (var (metadataPath, ilPathOpt) in generationDeltas)
            {
                generation = ReadDelta(metadataPath, ilPathOpt);
                if (generation == null)
                {
                    return 1;
                }

                generations.Add(generation);
                i++;
            }

            try
            {
                VisualizeGenerations(generations);
            }
            catch (BadImageFormatException e)
            {
                Console.WriteLine("Error reading metadata: " + e.Message);
                return 1;
            }

            return 0;
        }

        private static bool IsPEStream(Stream stream)
        {
            long oldPosition = stream.Position;
            bool result = stream.ReadByte() == 'M' && stream.ReadByte() == 'Z';
            stream.Position = oldPosition;
            return result;
        }

        private static GenerationData ReadBaseline(string peFilePath, bool embeddedPdb)
        {
            try
            {
                var stream = File.OpenRead(peFilePath);

                if (IsPEStream(stream))
                {
                    var peReader = new PEReader(stream);

                    if (embeddedPdb)
                    {
                        var embeddedEntries = peReader.ReadDebugDirectory().Where(entry => entry.Type == DebugDirectoryEntryType.EmbeddedPortablePdb).ToArray();
                        if (embeddedEntries.Length == 0)
                        {
                            throw new InvalidDataException("No embedded pdb found");
                        }

                        if (embeddedEntries.Length > 1)
                        {
                            throw new InvalidDataException("Multiple entries in Debug Directory Table of type EmbeddedPortablePdb");
                        }

                        var provider = peReader.ReadEmbeddedPortablePdbDebugDirectoryData(embeddedEntries[0]);

                        return new GenerationData(provider, provider.GetMetadataReader(), peReader);
                    }
                    else
                    {
                        return new GenerationData(peReader, peReader.GetMetadataReader(), peReader);
                    }
                }
                else if (IsManagedMetadata(stream))
                {
                    if (embeddedPdb)
                    {
                        throw new InvalidOperationException("File is not PE file");
                    }

                    var mdProvider = MetadataReaderProvider.FromMetadataStream(stream);
                    return new GenerationData(mdProvider, mdProvider.GetMetadataReader());
                }
                else
                {
                    throw new NotSupportedException("File format not supported");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error reading '{peFilePath}': {e.Message}");
                return null;
            }
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

      private void VisualizeGenerations(List<GenerationData> generations)
      {
          var skipGenerations = new HashSet<int>();
          List<MetadataReader> metadataReaders = generations.Select(g => g.MetadataReader).ToList();
          //var visualizer = new MetadataVisualizer(mdReaders, _writer);

          for (int generationIndex = 0; generationIndex < generations.Count; generationIndex++)
          {
              if (skipGenerations.Contains(generationIndex))
              {
                  continue;
              }

              var generation = generations[generationIndex];
              var mdReader = generation.MetadataReader;

              //if (generation.PEReaderOpt != null)
              //{
              //    VisualizeDebugDirectory(generation.PEReaderOpt, _writer);
              //}

              VisualizeHeaders(metadataReaders);

              if (generations.Count > 1)
              {
                  Console.WriteLine(">>>");
                  Console.WriteLine($">>> Generation {generationIndex}:");
                  Console.WriteLine(">>>");
                  Console.WriteLine();
              }

              //if (_arguments.DisplayMetadata)
              //{
              //    visualizer.Visualize(generationIndex);
              //}

              //if (_arguments.DisplayIL)
              //{
              //    VisualizeGenerationIL(visualizer, generationIndex, generation, mdReader);
              //}

              //VisualizeMemberRefs(mdReader);
          }
      }

      public void VisualizeHeaders(List<MetadataReader> metadataReaders)
      {
          var metadataReader = metadataReaders[0];

          Console.WriteLine($"MetadataVersion: {metadataReader.MetadataVersion}");

          if (metadataReader.DebugMetadataHeader != null)
          {
              Console.WriteLine("Id: " + BitConverter.ToString(metadataReader.DebugMetadataHeader.Id.ToArray()));

              if (!metadataReader.DebugMetadataHeader.EntryPoint.IsNil)
              {
                  Console.WriteLine($"EntryPoint: 0x{metadataReader.DebugMetadataHeader.EntryPoint}:x8");
              }
          }

          Console.WriteLine();
      }

  }

}
