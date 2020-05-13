using System.IO;
using System.Reflection.Metadata;

namespace test_pdb
{
    public static class Readers
    {
        public static bool IsPEStream(Stream stream)
        {
            long oldPosition = stream.Position;
            bool result = stream.ReadByte() == 'M' && stream.ReadByte() == 'Z';
            stream.Position = oldPosition;
            return result;
        }

        public static bool IsManagedMetadata(Stream stream)
        {
            long oldPosition = stream.Position;
            bool result = stream.ReadByte() == 'B' && stream.ReadByte() == 'S' && stream.ReadByte() == 'J' && stream.ReadByte() == 'B';
            stream.Position = oldPosition;
            return result;
        }

        public static FileStream GetFileStreamFromFilePath(string path)
        {
            return File.OpenRead(path);
        }

        public static MetadataReaderProvider GetMetadataReaderProviderFromFileStream(FileStream fileStream)
        {
            return MetadataReaderProvider.FromMetadataStream(fileStream);
        }

        public static MetadataReader GetMetadataReaderFromFileStream(FileStream stream)
        {
            var metadataReaderProvider = GetMetadataReaderProviderFromFileStream(stream);
            return GetMetadataReaderFromMetadataReaderProvider(metadataReaderProvider);
        }

        public static MetadataReader GetMetadataReaderFromMetadataReaderProvider(MetadataReaderProvider metadataReaderProvider)
        {
            return metadataReaderProvider.GetMetadataReader();
        }

        public static MetadataReaderProvider GetMetadataReaderProviderFromFilePath(string path)
        {
            var fileStream = GetFileStreamFromFilePath(path);
            return GetMetadataReaderProviderFromFileStream(fileStream);
        }

        public static MetadataReader GetMetadataReaderFromFile(string path)
        {
            var metadataReaderProvider = GetMetadataReaderProviderFromFilePath(path);
            return GetMetadataReaderFromMetadataReaderProvider(metadataReaderProvider);
        }

    }
}
