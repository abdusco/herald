using System.IO;

namespace Herald
{
    internal static class FileInfoExtensions
    {
        public static string ReadAsString(this FileInfo file)
        {
            using var sr = new StreamReader(file.OpenRead());
            return sr.ReadToEnd();
        }
    }
}