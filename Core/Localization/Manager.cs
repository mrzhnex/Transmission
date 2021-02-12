using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Core.Localization
{
    public class Manager
    {
        public Language Current { get; private set; } = Language.Default;
        public List<Language> Languages { get; private set; } = new List<Language>();
        private XmlSerializer XmlSerializer { get; set; } = new XmlSerializer(typeof(Language));

        public Manager()
        {
            CreateDirectory();
            LoadAll();
            SaveDefault();
            Current.CompareToNew(Languages.Last());
        }

        public string Translate(string word)
        {
            return Current.Translate(word);
        }

        public Language Load(string path)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                try { return (Language)XmlSerializer.Deserialize(fileStream); } catch (XmlException) { return null; }
            }
        }

        private void SaveDefault()
        {
            if (File.Exists(Info.GetFullFileName(Info.DefaultLanguageCulture)))
                return;
            using (FileStream fileStream = new FileStream(Info.GetFullFileName(Info.DefaultLanguageCulture), FileMode.OpenOrCreate))
            {
                try { XmlSerializer.Serialize(fileStream, Language.Default); } catch { }
            }
        }

        public void LoadAll()
        {
            Languages.Clear();
            
            string[] files = Directory.GetFiles(Info.GetFullFolderName());
            foreach (string tempFileName in files)
            {
                string[] fileName = tempFileName.Split('\\').Last().Split('.');
                if (fileName.Length != 2 || fileName[1] != Info.FileFormat || fileName[0].Length > Info.CultureMaxLength)
                    continue;
                Language language = Load(tempFileName);
                if (language != null)
                    Languages.Add(language);
            }
            if (Languages.Count == 0)
                Languages.Add(Language.Default);
        }

        private void CreateDirectory()
        {
            if (!Directory.Exists(Info.GetFullFolderName()))
                Directory.CreateDirectory(Info.GetFullFolderName());
        }
    }
}