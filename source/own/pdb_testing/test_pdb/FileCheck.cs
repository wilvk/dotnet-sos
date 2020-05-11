using System;
using System.Reflection.PortableExecutable;
using System.IO;

namespace test_pdb
{
    public static class FileCheck
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
    }
}
