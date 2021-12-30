using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Thismaker.Thoth.Manager.Wpf
{
    class LocalizationIO
    {
        public static async Task SaveAsync(Stream stream, LocalizationData data)
        {
            string json = System.Text.Json.JsonSerializer.Serialize(data);
            byte[] bytes = json.GetBytes<UTF8Encoding>();
            await stream.WriteAsync(bytes).ConfigureAwait(false);
        }

        public static async Task SaveAsync(string path, LocalizationData data)
        {
            FileStream stream = File.Open(path, FileMode.Create);
            await SaveAsync(stream, data);
        }

        public static async Task<LocalizationData> LoadAsync(Stream stream)
        {
            long len = stream.Length;
            byte[] buffer = new byte[len];
            int read = await stream.ReadAsync(buffer).ConfigureAwait(false);
            if (read < len)
            {
                Array.Resize(ref buffer, read);
            }

            string json = buffer.GetString<UTF8Encoding>();
            return System.Text.Json.JsonSerializer.Deserialize<LocalizationData>(json);
        }

        public static async Task<LocalizationData> LoadAsync(string path)
        {
            using FileStream fs = File.OpenRead(path);
            return await LoadAsync(fs).ConfigureAwait(false);
        }
    }
}
