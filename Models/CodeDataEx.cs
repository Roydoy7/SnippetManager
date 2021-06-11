using SnippetManager.Policies;
using System;

namespace SnippetManager.Models
{
    public static class CodeDataEx
    {
        public static void CreateFileName(this CodeData data)
        {
            data.FileName = Guid.NewGuid().ToString() + NamePolicy.Extension;
        }
    }
}
