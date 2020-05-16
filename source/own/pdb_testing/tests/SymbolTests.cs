using System;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Xunit;
using test_pdb;

namespace pdb_testing
{
    public class UnitTest1
    {
        [Fact]
        public void Test_CanGetMetaDataProviderFromStreamForPdb()
        {
            string peFilePath = Path.GetFullPath("../../../../../artefacts/test_debug.pdb");
            var file = File.OpenRead(peFilePath);
            var mdProvider = MetadataReaderProvider.FromMetadataStream(file);

            Assert.NotNull(mdProvider);
        }

        [Fact]
        public void Test_CanGetMetaDataReaderForPdb()
        {
            string peFilePath = Path.GetFullPath("../../../../../artefacts/test_debug.pdb");
            var file = File.OpenRead(peFilePath);
            var mdProvider = MetadataReaderProvider.FromMetadataStream(file);
            var mdReader = mdProvider.GetMetadataReader();

            Assert.NotNull(mdReader);
        }

        [Fact]
        public void Test_CanGetEntrypointAddressFromPdb()
        {
            string peFilePath = Path.GetFullPath("../../../../../artefacts/test_debug.pdb");
            var file = File.OpenRead(peFilePath);
            var mdProvider = MetadataReaderProvider.FromMetadataStream(file);
            var mdReader = mdProvider.GetMetadataReader();
            var address = Helpers.Address(mdReader, mdReader.DebugMetadataHeader.EntryPoint);

            Assert.Equal(100663297, address);
        }

        [Fact]
        public void Test_CanGetEntrypointAddressStringFromPdb()
        {
            string peFilePath = Path.GetFullPath("../../../../../artefacts/test_debug.pdb");
            var file = File.OpenRead(peFilePath);
            var mdProvider = MetadataReaderProvider.FromMetadataStream(file);
            var mdReader = mdProvider.GetMetadataReader();
            var address = Helpers.AddressString(mdReader, mdReader.DebugMetadataHeader.EntryPoint);

            Assert.Equal("0x06000001", address);
        }

        [Fact]
        public void Test_CanGetEntrypointAdressTableStringFromPdb()
        {
            string peFilePath = Path.GetFullPath("../../../../../artefacts/test_debug.pdb");
            var file = File.OpenRead(peFilePath);
            var mdProvider = MetadataReaderProvider.FromMetadataStream(file);
            var mdReader = mdProvider.GetMetadataReader();
            var table = Helpers.Table(mdReader.DebugMetadataHeader.EntryPoint);

            Assert.Equal("MethodDef", table);
        }

        [Fact]
        public void Test_CanGetCorrectNumberOfMethodsFromMethodDebugInformation()
        {
            string path = Path.GetFullPath("../../../../../artefacts/test_debug.pdb");
            var metadataReader = Readers.GetMetadataReaderFromFile(path);
            var methodDebugInformation = Symbols.GetMethodDebugInformationList(metadataReader);

            Assert.Equal(3, methodDebugInformation.Count);;
        }

        [Fact]
        public void Test_CanGetCorrectNumberOfDocuments()
        {
            string path = Path.GetFullPath("../../../../../artefacts/test_debug.pdb");
            var metadataReader = Readers.GetMetadataReaderFromFile(path);
            var documents = Symbols.GetDocuments(metadataReader);

            Assert.Equal(3, documents.Count);
        }

        [Fact]
        public void Test_CanGetCorrectDocumentName()
        {
            string path = Path.GetFullPath("../../../../../artefacts/test_debug.pdb");
            var metadataReader = Readers.GetMetadataReaderFromFile(path);
            var documents = Symbols.GetDocuments(metadataReader);
            var expectedDocumentName = "Program.cs";

            var actualDocumentName = Helpers.DocumentName(metadataReader, documents[0].Name);
            var actualDocumentNameTrimmed = Path.GetFileName(actualDocumentName);

            Assert.Equal(expectedDocumentName, actualDocumentNameTrimmed);
        }

        [Fact]
        public void Test_CanGetCorrectNumberOfSequencePointsForMethod()
        {
            string path = Path.GetFullPath("../../../../../artefacts/test_debug.pdb");
            var metadataReader = Readers.GetMetadataReaderFromFile(path);

            var methodDebugInformationList = Symbols.GetMethodDebugInformationList(metadataReader);
            var sequencePointsList = Symbols.GetSequencePointsFromMethodDebugInformation(methodDebugInformationList[0]);

            Assert.Equal(9, sequencePointsList.Count);
        }

        [Fact]
        public void Test_CanGetCorrectDocumentRowNumber()
        {
            string path = Path.GetFullPath("../../../../../artefacts/test_debug.pdb");
            var metadataReader = Readers.GetMetadataReaderFromFile(path);

            var documentHandles = Symbols.GetDocumentHandles(metadataReader);

            var documentRowNumber = Symbols.GetDocumentRowNumber(metadataReader, documentHandles[1]);

            Assert.Equal(2, documentRowNumber);
        }

        [Fact]
        public void Test_CanGetCorrectMethodRowNumber()
        {
            string path = Path.GetFullPath("../../../../../artefacts/test_debug.pdb");
            var metadataReader = Readers.GetMetadataReaderFromFile(path);

            var methodDebugInformationHandles = Symbols.GetMethodDebugInformationHandles(metadataReader);

            var rowNumber = Symbols.GetMethodRowNumber(methodDebugInformationHandles[2]);

            Assert.Equal(3, rowNumber);
        }
    }
}
