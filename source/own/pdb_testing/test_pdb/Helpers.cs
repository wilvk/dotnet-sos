using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

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

        public static string DocumentName(MetadataReader reader, DocumentNameBlobHandle handle)
        {
            return EscapeNonPrintableCharacters(reader.GetString(handle));
        }

        public static string EscapeNonPrintableCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in str)
            {
                bool escape;
                switch (CharUnicodeInfo.GetUnicodeCategory(c))
                {
                    case UnicodeCategory.Control:
                    case UnicodeCategory.OtherNotAssigned:
                    case UnicodeCategory.ParagraphSeparator:
                    case UnicodeCategory.Surrogate:
                        escape = true;
                        break;

                    default:
                        escape = c >= 0xFFFC;
                        break;
                }

                if (escape)
                {
                    sb.AppendFormat("\\u{0:X4}", (int)c);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }


//        private string Literal(BlobHandle getHandle, BlobKind kind, Func<MetadataReader, BlobHandle, string> getValue)
//        {
//            BlobHandle handle;
//            try
//            {
//                handle = getHandle();
//            }
//            catch (BadImageFormatException)
//            {
//                return BadMetadataStr;
//            }
//
//            if (!handle.IsNil && kind != BlobKind.None)
//            {
//                _blobKinds[handle] = kind;
//            }
//
//            return Literal(handle, (r, h) => getValue(r, (BlobHandle)h));
//        }
//
//        private string Literal(Handle handle, Func<MetadataReader, Handle, string> getValue)
//        {
//            if (handle.IsNil)
//            {
//                return "nil";
//            }
//
//            if (_aggregator != null)
//            {
//                Handle generationHandle = _aggregator.GetGenerationHandle(handle, out int generation);
//
//                var generationReader = _readers[generation];
//                string value = GetValueChecked(getValue, generationReader, generationHandle);
//                int offset = generationReader.GetHeapOffset(handle);
//                int generationOffset = generationReader.GetHeapOffset(generationHandle);
//
//                if (NoHeapReferences)
//                {
//                    return value;
//                }
//                else if (offset == generationOffset)
//                {
//                    return $"{value} (#{offset:x})";
//                }
//                else
//                {
//                    return $"{value} (#{offset:x}/{generationOffset:x})";
//                }
//            }
//
//            if (IsDelta)
//            {
//                // we can't resolve the literal without aggregate reader
//                return $"#{_reader.GetHeapOffset(handle):x}";
//            }
//
//            int heapOffset = MetadataTokens.GetHeapOffset(handle);
//
//            // virtual heap handles don't have offset:
//            bool displayHeapOffset = !NoHeapReferences && heapOffset >= 0;
//
//            return $"{GetValueChecked(getValue, _reader, handle):x}" + (displayHeapOffset ? $" (#{heapOffset:x})" : "");
//        }


    }
}
