using SnippetManager.Models;
using SnippetManager.Policies;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SnippetManager.Components
{
    public static class CodeDataMethods
    {
        private static string FolderPath => PathPolicy.FolderPath;
        public static void ModifyCodeData(CodeData data)
        {
            var filePath = Path.Combine(FolderPath, data.FileName);
            var jsonStr = JsonSerializer.Serialize(data);
            File.WriteAllText(filePath, jsonStr);
        }

        public static void DeleteCodeDataFile(string fileName)
        {
            var filePath = Path.Combine(FolderPath, fileName);
            File.Delete(filePath);
        }

        public static void CreateCodeDataFile(CodeData data)
        {
            data.CreateFileName();
            var filePath = Path.Combine(FolderPath, data.FileName);
            var jsonStr = JsonSerializer.Serialize(data);
            File.WriteAllText(filePath, jsonStr);
        }

        public static IEnumerable<CodeData> GetCodeDatas()
        {
            foreach (var filePath in Directory.GetFiles(FolderPath, "*" + NamePolicy.Extension))
            {
                yield return GetCodeData(filePath);
            }
        }

        public static CodeData GetCodeData(string filePath)
        {
            var jsonStr = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<CodeData>(jsonStr);
        }

        public static void EnsureFolderExist()
        {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);
        }
    }
}
