using System.Collections.Generic;
using System.IO;

namespace ReencodeTool
{
    interface IContext
    {
        DirectoryInfo DirFFME { get; }
        string FfmpegPath { get; }
        string FfprobePath { get; }
        string TranscodeFolder { get; }
        List<FileInfo> OriginalFiles { get; }
    }
}
