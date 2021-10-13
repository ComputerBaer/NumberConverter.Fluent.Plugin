using System;

namespace NumberConverter.Fluent.Plugin
{
    [Flags]
    public enum ConversionType
    {
        None = 0,
        Any = -1,

        Bin = 0x1,
        Oct = 0x2,
        Dec = 0x4,
        Hex = 0x8
    }
}
