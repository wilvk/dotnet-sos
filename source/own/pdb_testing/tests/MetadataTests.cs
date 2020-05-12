using System;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Xunit;
using test_pdb;

namespace pdb_testing
{
    public class MetadataTests
    {
        [Fact]
        public void Test_CanGetMetaDataFromDll()
        {
            string peFilePath = Path.GetFullPath("../../../../../artefacts/test_debug.dll");
            var file = File.OpenRead(peFilePath);
            var peReader = new PEReader(file);
            var metaDataReader = peReader.GetMetadataReader();

            Assert.NotNull(metaDataReader);
        }

        [Fact]
        public void Test_GetMetaDataFromPdbThrowsException()
        {
            string peFilePath = Path.GetFullPath("../../../../../artefacts/test_debug.pdb");
            var file = File.OpenRead(peFilePath);
            var peReader = new PEReader(file);

            Assert.Throws<BadImageFormatException>(() => peReader.GetMetadataReader());
        }
    }
}
