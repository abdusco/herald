using System.IO;
using System.Reflection;

namespace Herald
{
    internal static class AssemblyExtensions
    {
        public static string GetEmbeddedResource(this Assembly assembly, string path)
        {
            var stream = assembly.GetManifestResourceStream(path);
            if (stream == null)
            {
                throw new FileNotFoundException($"Embedded resource not found in {assembly.FullName}", path);
            }

            using var sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }
    }
}