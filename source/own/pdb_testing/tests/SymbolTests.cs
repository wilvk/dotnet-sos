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
    }
}
