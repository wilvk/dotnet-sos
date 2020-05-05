using System;
using System.IO;
using Microsoft.DiaSymReader;
using Microsoft.DiaSymReader.PortablePdb;
using System.Reflection;

namespace test_pdb
{
    class Program
    {
        static void Main(string[] args)
        {
            var symBinder = new SymBinder();

            //string dllFilename = "../../artefacts/test_debug.dll";
            //var dllStream = new MemoryStream(GetResourceBlob(dllFilename));

            string pdbFilename = "../../artefacts/test_debug.pdb";
            var pdbStream = new MemoryStream(GetResourceBlob(pdbFilename));

            //var test = symBinder.GetReaderFromStream(pdbStreeam,
            //SymReader reader = symBinder.GetReaderFromStream(dllStream, pdbStream);

            ISymUnmanagedReader symReader;
            var hresult = symBinder.GetReaderFromPdbFile(NotImplementedMetadataProvider.Instance, pdbFilename, out symReader);

            Console.WriteLine("Hello World!");
        }

        private static Stream GetResourceStream(string name)
        {
            //var assembly = typeof(Program).GetTypeInfo().Assembly;
            var assembly =  MethodBase.GetCurrentMethod().DeclaringType.Assembly;

            var stream = assembly.GetManifestResourceStream(name);
            if (stream == null)
            {
                throw new InvalidOperationException($"Resource '{name}' not found in {assembly.FullName}.");
            }

            return stream;
        }

        private static byte[] GetResourceBlob(string name)
        {
            using (var stream = GetResourceStream(name))
            {
                var bytes = new byte[stream.Length];
                using (var memoryStream = new MemoryStream(bytes))
                {
                    stream.CopyTo(memoryStream);
                }

                return bytes;
            }
        }
    }

    sealed class NotImplementedMetadataProvider : IMetadataImportProvider
    {
        public static readonly IMetadataImportProvider Instance = new NotImplementedMetadataProvider();

        public object GetMetadataImport()
        {
            throw new NotImplementedException();
        }
    }

    sealed class TestMetadataProvider : IMetadataImportProvider
    {
        private readonly Func<IMetadataImport> _importProvider;

        public TestMetadataProvider(Func<IMetadataImport> importProvider)
        {
            _importProvider = importProvider;
        }

        public object GetMetadataImport() => _importProvider();
    }

}
