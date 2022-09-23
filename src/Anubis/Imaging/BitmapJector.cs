using System;
using System.Collections.Generic;
using SkiaSharp;
using System.Collections;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Thismaker.Anubis.Imaging
{
    /// <summary>
    /// A <see cref="Jector"/> with functionality for performing steganographic operations on bitmap image files.
    /// </summary>
    /// <remarks>
    /// While the input image can be of one of the many bitamp formats(PNG, JPEG, GIF, BMP, TIFF),
    /// the resulting image is always a PNG image. This is so because the PNG format is not a lossy compression format,
    /// which is important if the data is to be retained.
    /// </remarks>
    public class BitmapJector : Jector
    {
        /// <summary>
        /// Get or sets a value determining whether the alpha channel of the image is written to.
        /// </summary>
        /// <remarks>
        /// By default, the alpha channel is not written to when hiding the data.
        /// However, setting the value to true allows more data to be written, increasing the capacity of the image, but may produce more artefacts in the output image.
        /// </remarks>
        public bool UseAlphaChannel { get; set; }

        ///<inheritdoc/>
        public override Task InjectAsync(Stream source, Stream destination, Stream data, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            SKBitmap inputImage = SKBitmap.Decode(source);

            int width = inputImage.Width;
            int height = inputImage.Height;

            List<byte> ls_data = new List<byte>();

            ls_data.AddRange(data.ReadAllBytes());
            
            if(Eof != null)
            {
                ls_data.AddRange(Eof);
            }

            BitArray message = new BitArray(ls_data.ToArray());

            int count = message.Count;

            if (EnsureSuccess)
            {
                int maxWritable = (UseAlphaChannel ? 4 : 3) * LsbDepth * width * height;
                
                if (count > maxWritable)
                {
                    throw new InsufficientSpaceException("A successful write cannot be ensured because the data is too large");
                }
                    
            }

            SKBitmap outputImage = inputImage.Copy();

            int pos = 0;

            for (int y = 0; y < height; y++)
            {
                bool stop = false;
                for (int x = 0; x < width; x++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    if (pos == count)
                    {
                        stop = true;
                        break;
                    }

                    SKColor pixel = inputImage.GetPixel(x, y);

                    int[] vals = new int[] { pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue };

                    for (int v = 0; v < vals.Length; v++)
                    {
                        if (!UseAlphaChannel && v == 0) continue;

                        BitArray bitArray = new BitArray(new[] { vals[v] });
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

                        vals[v] = (int)bitArray.ToULong();
                        if (stop) break;
                    }

                    pixel=new SKColor((byte)vals[1], (byte)vals[2], (byte)vals[3], (byte)vals[0]);

                    outputImage.SetPixel(x, y, pixel);

                    if (progress != null)
                    {
                        progress.Report(pos / (double)count);
                    }
                }

                if (stop) break;
            }
            
            outputImage.Encode(destination, SKEncodedImageFormat.Png, 100);
            
            return Task.CompletedTask;
        }
        
        ///<inheritdoc/>
        public override Task EjectAsync(Stream source, Stream destination, IProgress<double> progress=null, CancellationToken cancellationToken=default)
        {
            SKBitmap inputImage =SKBitmap.Decode(source);

            int height = inputImage.Height;
            int width = inputImage.Width;

            int count = (width * height) * LsbDepth * (UseAlphaChannel ? 4 : 3);
            BitArray message = new BitArray(count, false);
            int pos = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    SKColor pixel = inputImage.GetPixel(x, y);

                    byte[] vals = UseAlphaChannel ? new byte[] { pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue }
                    : new byte[] { pixel.Red, pixel.Green, pixel.Blue }; ;

                    foreach (byte val in vals)
                    {
                        BitArray bitArray = new BitArray(new[] { val });
                        for (int i = 0; i < LsbDepth; i++)
                        {
                            message[pos] = bitArray[i];
                            pos++;
                        }
                    }

                    if (progress != null)
                    {
                        progress.Report(pos / (double)count);
                    }
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