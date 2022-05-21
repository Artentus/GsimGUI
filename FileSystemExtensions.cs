using System.IO;

namespace GsimGUI
{
    internal static class FileSystemExtensions
    {
        public static string NameWithoutExtension(this FileInfo file)
            => Path.GetFileNameWithoutExtension(file.Name);
    }
}
