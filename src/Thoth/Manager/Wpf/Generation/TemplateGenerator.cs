using System.IO;
using System.Text;

namespace Thismaker.Thoth.Manager.Wpf
{
    public static class TemplateGenerator
    {
        public static void GenerateTemplate(LocalizationData data, string outputDirectory, string namespaceVal)
        {
            var assembly = typeof(TemplateGenerator).Assembly;
            string itemTemplate, classTemplate;
            using var itemTemplateStream = assembly.GetManifestResourceStream("Thismaker.Thoth.Manager.Wpf.Generation.ItemTemplate.txt");
            using var itemTemplateReader = new StreamReader(itemTemplateStream);
            itemTemplate = itemTemplateReader.ReadToEnd();

            using var classTemplateStream = assembly.GetManifestResourceStream("Thismaker.Thoth.Manager.Wpf.Generation.ClassTemplate.txt");
            var classTemplateReader = new StreamReader(classTemplateStream);
            classTemplate = classTemplateReader.ReadToEnd();

            classTemplate = classTemplate.Replace("[NAMESPACE]", namespaceVal);

            foreach(var table in data.Tables)
            {
                string result = classTemplate.Replace("[TABLE_NAME]", table.Key);

                StringBuilder body = new();

                foreach(var item in table.Value.Items)
                {
                    string itemValue = itemTemplate.Replace("[ITEM_KEY]", item.Key);
                    body.Append(itemValue);
                }

                result = result.Replace("[BODY]", body.ToString());
                string classOutputPath = Path.Combine(outputDirectory, $"{table.Key}.cs");
                using var classStream = File.Create(classOutputPath);

                using StreamWriter classWriter = new(classStream);
                classWriter.Write(result);
            }
        }
    }
}
