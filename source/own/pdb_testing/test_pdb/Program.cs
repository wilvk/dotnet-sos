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
//            var symBinder = new SymBinder();

            //string dllFilename = "../../artefacts/test_debug.dll";
            //var dllStream = new MemoryStream(GetResourceBlob(dllFilename));

//            string pdbFilename = "test_debug.pdb";
//            var pdbPath = Path.GetFullPath(pdbFilename);
//            Console.WriteLine("path: " + pdbPath.ToString());
//            var pdbStream = new MemoryStream(GetResourceBlob(pdbFilename));

            //var test = symBinder.GetReaderFromStream(pdbStreeam,
            //SymReader reader = symBinder.GetReaderFromStream(dllStream, pdbStream);

//            ISymUnmanagedReader symReader;
//            var hresult = symBinder.GetReaderFromPdbFile(NotImplementedMetadataProvider.Instance, pdbFilename, out symReader);

//            int documentCount = 0;

//            hresult = symReader.GetDocuments(0, out documentCount, null);
//            Console.WriteLine("Number of documents: " + documentCount.ToString());

//            ISymUnmanagedDocument[] documents = new ISymUnmanagedDocument[documentCount];
//            hresult = symReader.GetDocuments(0, out documentCount, documents);

//            Assert.Equal(HResult.S_OK, document.GetUrl(0, out actualCount, null));
//
//            char[] actualUrl = new char[actualCount];
//            Assert.Equal(HResult.S_OK, document.GetUrl(actualCount, out actualCount2, actualUrl));

            var symBinder = new SymBinder();

            var importer = new TestIMetadataImport(new MemoryStream(TestResources.Documents.PortableDll));

            string peFilePath = Path.GetFullPath("../../artefacts/test_debug.dll");
            string pdbFilePath = Path.GetFullPath("../../artefacts/test_debug.pdb");

            string searchPath = null;

            ISymUnmanagedReader symReader;
            symBinder.GetReaderForFile(importer, peFilePath, searchPath, out symReader));

            int actualCount;
            symReader.GetDocuments(0, out actualCount, null));

            // check that metadata import hasn't been disposed:
            ((SymReader)symReader).GetMetadataImport());

            ((ISymUnmanagedDispose)symReader).Destroy();

            Console.WriteLine("Hello World!");
        }

//        private static byte[] GetResourceBlob(string name)
//        {
//            var assembly =  MethodBase.GetCurrentMethod().DeclaringType.Assembly;
//            var stream = assembly.GetManifestResourceStream(name);
//            Console.WriteLine("assembly: " + assembly);
//
//            if (stream == null)
//            {
//                throw new InvalidOperationException($"Resource '{name}' not found in {assembly.FullName}.");
//            }
//            var bytes = new byte[stream.Length];
//            using (var memoryStream = new MemoryStream(bytes))
//            {
//                stream.CopyTo(memoryStream);
//            }
//
//            return bytes;
//        }
//    }
//
//    sealed class NotImplementedMetadataProvider : IMetadataImportProvider
//    {
//        public static readonly IMetadataImportProvider Instance = new NotImplementedMetadataProvider();
//
//        public object GetMetadataImport()
//        {
//            throw new NotImplementedException();
//        }
//    }
//
//    sealed class TestMetadataProvider : IMetadataImportProvider
//    {
//        private readonly Func<IMetadataImport> _importProvider;
//
//        public TestMetadataProvider(Func<IMetadataImport> importProvider)
//        {
//            _importProvider = importProvider;
//        }
//
//        public object GetMetadataImport() => _importProvider();
//    }

}
