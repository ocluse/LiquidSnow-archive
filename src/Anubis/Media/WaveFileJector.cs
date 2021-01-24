using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Anubis.Media
{
    public class WaveFileJector : IJector
    {
        public string EOF { get; set; } = "#$%-";
        
        public bool EnsureSuccess { get; set; } = false;

        public byte[] Eject(object source)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> EjectAsync(object source, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task EjectAsync(Stream source, Stream destination, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public object Inject(object source, byte[] data)
        {
            throw new NotImplementedException();
        }

        public Task<object> InjectAsync(object source, byte[] data, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task InjectAsync(Stream source, Stream destination, Stream data, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            var inputWave = new WaveFile(source);
            inputWave.Save(destination);
            return null;

        }
    }
}
