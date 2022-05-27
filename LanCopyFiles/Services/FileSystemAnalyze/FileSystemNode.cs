using System.Collections.Generic;

namespace LanCopyFiles.Services.FileSystemAnalyze;

public class FileSystemNode
{
    public FileSystemType Type { get; set; }
    public string Name { get; set; }
    public List<FileSystemNode> Children { get; set; }
}