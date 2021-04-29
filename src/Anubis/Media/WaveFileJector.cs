using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Anubis.Media
{
    public class WaveFileJector : Jector
    {
        public override Task InjectAsync(Stream source, Stream destination, Stream data, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            using var input = new WaveFile(source);
            using var output = new WaveFile(destination);
            output.DuplicateFormat(input);

            var ls_data = new List<byte>(data.ReadAllBytes());

            if (!string.IsNullOrEmpty(EOF))
            {
                ls_data.AddRange(Sign);
            }

            var message = new BitArray(ls_data.ToArray());

            var count = message.Length;
            var maxData= input.DataSize * input.Format.NumChannels * LsbDepth;
            if (EnsureSuccess)
            {
                if(count>maxData)
                    throw new InvalidOperationException("There is not enough room in the audio file to write the data");
            }

            var pos = 0;
            while (true)
            {
                bool stop = false;

                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                var sample = input.GetNextSample();

                if (sample == null) break;

                foreach(var channel in sample.Channels)
                {
                    if (stop == true) break;

                    var bitArray = new BitArray(channel.Data);

                    for (int i = 0; i < LsbDepth; i++)
                    {
                        if (pos == count)
                        {
                            stop = true;
                            break;
                        }

                        bitArray[i] = message[pos];
                        pos++;
                    }

                    channel.Data = bitArray.ToBytes();
                }

                output.WriteSample(sample);

                if (progress != null)
                {
                    var percent = pos / (float)maxData;
                    progress.Report(percent);
                }

                //if (stop) break;
            }

            return Task.CompletedTask;
        }

        public override Task EjectAsync(Stream source, Stream destination, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            using var input = new WaveFile(source);
            var count = input.DataSize * input.Format.NumChannels * LsbDepth;
            var pos = 0;
            var message = new BitArray(count, false);
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                var sample = input.GetNextSample();
                if (sample == null) break;

                foreach(var channel in sample.Channels)
                {
                    var bitArray = new BitArray(channel.Data);
                    for(int i = 0; i < LsbDepth; i++)
                    {
                        message[pos] = bitArray[i];
                        pos++;
                    }
                }

                if (progress != null)
                {
                    progress.Report(pos / (float)input.DataSize);
                }
            }

            var bytes = message.ToBytes();
            List<byte> result;

            if (string.IsNullOrEmpty(EOF))
            {
                result = new List<byte>(bytes);
            }
            else
            {
                result = new List<byte>();
                var success = false;

                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] == Sign[0])
                    {
                        //check sequence
                        if (IsSignature(bytes, i))
                        {
                            success = true;
                            break;
                        }
                    }
                    result.Add(bytes[i]);
                }

                if (EnsureSuccess && !success)
                    throw new EndOfFileException("Failed to locate the specified EOF");
            }

            destination.Position = 0;
            destination.Write(result.ToArray(), 0, result.Count);
            destination.Flush();
            return Task.CompletedTask;
        }
    }
}
