using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Thismaker.Anubis.Media
{
    /// <summary>
    /// A <see cref="Jector"/> with functionality to perform steganographic operations on WAVE files
    /// </summary>
    /// <remarks>
    /// This class only handles simple WAVE files that strictly follow the RIFF format.
    /// Any additional data not specified in the RIFF standard may be discarded in the final output.
    /// </remarks>
    public class WaveFileJector : Jector
    {
        ///<inheritdoc/>
        public override Task InjectAsync(Stream source, Stream destination, Stream data, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            using WaveFile input = new WaveFile(source);
            using WaveFile output = new WaveFile(destination);
            output.DuplicateFormat(input);

            List<byte> ls_data = new List<byte>(data.ReadAllBytes());

            if (Eof != null)
            {
                ls_data.AddRange(Eof);
            }

            BitArray message = new BitArray(ls_data.ToArray());

            int count = message.Length;
            int maxData = input.DataSize * input.Format.NumChannels * LsbDepth;
            if (EnsureSuccess)
            {
                if(count>maxData)
                    throw new InsufficientSpaceException("A successful write cannot be ensured because the data is too large");
            }

            int pos = 0;
            
            while (true)
            {
                bool stop = false;

                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                Sample sample = input.GetNextSample();

                if (sample == null) break;

                foreach(Channel channel in sample.Channels)
                {
                    if (stop == true) break;

                    BitArray bitArray = new BitArray(channel.Data);

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
                    progress.Report(pos / (double)maxData);
                }
            }

            return Task.CompletedTask;
        }

        ///<inheritdoc/>
        public override Task EjectAsync(Stream source, Stream destination, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            using WaveFile input = new WaveFile(source);
            int count = input.DataSize * input.Format.NumChannels * LsbDepth;
            int pos = 0;
            BitArray message = new BitArray(count, false);
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                Sample sample = input.GetNextSample();
                if (sample == null) break;

                foreach(Channel channel in sample.Channels)
                {
                    BitArray bitArray = new BitArray(channel.Data);
                    for(int i = 0; i < LsbDepth; i++)
                    {
                        message[pos] = bitArray[i];
                        pos++;
                    }
                }

                if (progress != null)
                {
                    progress.Report(pos / (double)input.DataSize);
                }
            }

            byte[] bytes = message.ToBytes();
            List<byte> result;

            if (Eof == null)
            {
                result = new List<byte>(bytes);
            }
            else
            {
                result = new List<byte>();
                bool success = false;

                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] == Eof[0])
                    {
                        if (IsEndOfFileSequence(bytes, i))
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