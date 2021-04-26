using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Thismaker.Thoth
{
    public class LocalizationData
    {
        public Dictionary<string, LocalizationTable> Tables { get; set; }

        public List<Locale> Locales { get; set; }

        public string DefaultLocaleId { get; set; }

        public string DefaultTableKey { get; set; }

        public static async Task<LocalizationData> LoadAsync(Stream stream)
        {
            return await JsonSerializer.DeserializeAsync<LocalizationData>(stream);
        }

        public async Task SaveAsync(Stream stream)
        {
            using var msData = new MemoryStream();
            var json = JsonSerializer.Serialize(this);
            await JsonSerializer.SerializeAsync(stream, this);
        }

        public async Task SaveAsync(string path)
        {
            var stream = File.OpenWrite(path);
            await SaveAsync(stream);
        }
    }
}
