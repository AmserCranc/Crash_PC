using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
unsafe public struct NSF
{
    const int MAX_NSF_SIZE = 10_000_000;
    [FieldOffset(0)] public fixed byte raw[MAX_NSF_SIZE];


    public NSF(string streamPath)
    {
        
    }
}
