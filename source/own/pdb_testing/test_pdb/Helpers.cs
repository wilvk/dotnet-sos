using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace test_pdb
{
    public static class Helpers
    {
        public static string Table(Handle handle)
        {
            MetadataTokens.TryGetTableIndex(handle.Kind, out var table);
            return table.ToString();
        }

        public static int Address(MetadataReader reader, Handle handle)
        {
            return reader.GetToken(handle);
        }

        public static string AddressString(MetadataReader reader, Handle handle)
        {
            var address = Address(reader, handle);
            return $"0x{address:x8}";
        }
    }
}
