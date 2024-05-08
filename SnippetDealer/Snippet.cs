using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Snippets
{
    public class Snippet
    {
        #region Fields and Properties
        private string _name;
        private string _snippetFileName;
        private string _description;
        private string _category;
        private string _text;

        public string Name { get => _name; set => _name = value; }
        public string SnippetFileName { get => _snippetFileName; set => _snippetFileName = value; }
        public string Description { get => _description; set => _description = value; }
        public string Category { get => _category; set => _category = value; }
        public string Text { get => _text; set => _text = value; }
        #endregion

        public Snippet() { }

        public string BuildSnippetFileName()
        {
            var prefix = "";
            switch (Category)
            {
                case "C#":
                    prefix = "code";
                    break;
                case "WPF":
                    prefix = "wpf";
                    break;
                case "SQL":
                    prefix = "sql";
                    break;
                case "Programs":
                    prefix = "programs";
                    break;
            }
            return $"{prefix}.{SnippetFileName.ToLower()}.snippet";
        }

        public void Save(string definitionsFile)
        {
            var folder = $"{Path.GetDirectoryName(definitionsFile)}{Path.DirectorySeparatorChar}";            
            File.WriteAllText($"{folder}{SnippetFileName}", Text);
        }

        public string GetContent(string folder)
        {
            var fileContent = File.ReadAllLines($"{folder}{Path.DirectorySeparatorChar}{SnippetFileName}");
            var content = "";
            for (int i = 0; i < fileContent.Length; i++)
            {
                content = $"{content}{Environment.NewLine}{fileContent[i]}";
            }
            return content;
        }

        public static void SaveList(List<Snippet> snippetList, FileInfo definitionsFile)
        {
            Directory.CreateDirectory(definitionsFile.Directory.FullName);
            if (!File.Exists(definitionsFile.FullName))
            {
                var fileStream = File.Create(definitionsFile.FullName);
                fileStream.Close();
            }

            string json = JsonConvert.SerializeObject(snippetList, Formatting.Indented);
            File.WriteAllText(definitionsFile.FullName, json);
        }

        public static List<Snippet> ReadCollection(FileInfo definitionsFile)
        {
            return JsonConvert.DeserializeObject<List<Snippet>>(File.ReadAllText(definitionsFile.FullName));
        }
    }
}
