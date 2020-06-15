using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace Microsoft.Diagnostics.Runtime
{
    public sealed class RefCountedFreeLibrary
    {
        private readonly IntPtr _library;
        private int _refCount;

        public RefCountedFreeLibrary(IntPtr library)
        {
            _library = library;
            _refCount = 1;
        }

        public int AddRef()
        {
            return Interlocked.Increment(ref _refCount);
        }

        public int Release()
        {
            int count = Interlocked.Decrement(ref _refCount);
            if (count == 0 && _library != IntPtr.Zero)
              NativeLibrary.Free(_library);

            return count;
        }
    }
}
