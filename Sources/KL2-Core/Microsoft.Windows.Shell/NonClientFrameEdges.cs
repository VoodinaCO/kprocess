using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Windows.Shell
{
    [Flags]
    public enum NonClientFrameEdges
    {
        Bottom = 8,
        Left = 1,
        None = 0,
        Right = 4,
        Top = 2
    }
}
