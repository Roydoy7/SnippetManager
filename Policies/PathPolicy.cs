using SnippetManager.Components;
using System.IO;

namespace SnippetManager.Policies
{
    public static class PathPolicy
    {
        public static string FolderName => "SnippletManager";
        public static string FolderPath
        {
            get
            {
                return Path.Combine(PathEx.GetLocalAppDataPath(), FolderName);
            }
        }
    }
}
