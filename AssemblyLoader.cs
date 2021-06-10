using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SnippetManager
{
    public static partial class AssemblyLoader
    {
        public static void LoadAssembly()
        {
            var folderPath = AssemblyPath.GetAssemblyPath();
            foreach (var assemblyName in AssemblyNames)
                Assembly.LoadFrom(Path.Combine(folderPath, assemblyName));
        }

        public static List<string> AssemblyNames = new List<string> {
            "Microsoft.Xaml.Behaviors.dll",
        };
    }
}
