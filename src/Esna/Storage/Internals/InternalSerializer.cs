using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Thismaker.Horus.IO;

namespace Thismaker.Esna
{
    internal class InternalSerializer : ISerializer
    {
        private static InternalSerializer _instance;
        public static InternalSerializer Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new InternalSerializer();
                }

                return _instance;
            }
        }

        public Task<T> DeserializeAsync<T>(Stream sourceStream)
        {
            string json = sourceStream.ReadAllBytes().GetString<UTF8Encoding>();

            T result = JsonConvert.DeserializeObject<T>(json);

            return Task.FromResult(result);
        }

        public Task SerializeAsync<T>(T data, Stream destinationStream)
        {
            string json = JsonConvert.SerializeObject(data);

            byte[] buffer = json.GetBytes<UTF8Encoding>();

            destinationStream.Write(buffer, 0, buffer.Length);

            return Task.CompletedTask;
        }
    }
}
