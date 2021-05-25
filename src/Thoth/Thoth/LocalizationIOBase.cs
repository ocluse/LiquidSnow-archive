using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Thismaker.Thoth
{

    public abstract class LocalizationIOBase
    {
        public async Task SaveAsync(Stream stream, LocalizationData data)
        {
            var json = Serialize(data);
            var bytes = json.GetBytes<UTF8Encoding>();
            await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
        }

        public async Task SaveAsync(string path, LocalizationData data)
        {
            var stream = File.Open(path, FileMode.Create);
            await SaveAsync(stream, data);
        }

        public async Task<LocalizationData> LoadAsync(Stream stream)
        {
            var len = stream.Length;
            var buffer = new byte[len];
            var read = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            if (read < len)
            {
                Array.Resize(ref buffer, read);
            }

            var json = buffer.GetString<UTF8Encoding>();
            return Deserialize(json);
        }

        public async Task<LocalizationData> LoadAsync(string path)
        {
            using var fs = File.OpenRead(path);
            return await LoadAsync(fs).ConfigureAwait(false);
        }

        protected abstract string Serialize(LocalizationData data);

        protected abstract LocalizationData Deserialize(string json);
    }
}