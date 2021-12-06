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
    /// A Jector that allows for writing data into and from <see cref="Bitmap"/> 
    /// image files.
    /// </summary>
    public class BitmapJector : Jector
    {
        /// <summary>
        /// When <see cref="true"/>, the Alpha Channel will also be written to, 
        /// and read from,
        /// allowing for writing more data into the image. 
        /// However, the resultant image may have artifacts.
        /// </summary>
        public bool UseAlpha { get; set; } = false;

        public override Task InjectAsync(Stream source, Stream destination, Stream data, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            //Prepare the images:
            SKBitmap inputImage = SKBitmap.Decode(source);

            int width = inputImage.Width;
            int height = inputImage.Height;

            List<byte> ls_data = new List<byte>();

            //Read the data to be injected
            ls_data.AddRange(data.ReadAllBytes());
            //add the EOF:
            if (!string.IsNullOrEmpty(EOF))
            {
                ls_data.AddRange(EOFBytes);
            }

            //create the message:
            BitArray message = new BitArray(ls_data.ToArray());

            int count = message.Count;

            if (EnsureSuccess)
            {
                int maxWritable = (UseAlpha ? 4 : 3) * LsbDepth * width * height;
                if (count > maxWritable)
                    throw new InvalidOperationException("There is not enough room in the picture to write the data");
            }

            //create the output image:
            SKBitmap outputImage = inputImage.Copy();
            //var outputImage = inputImage.Clone(new Rectangle(0, 0, inputImage.Width, inputImage.Height), inputImage.PixelFormat);

            int pos = 0;

            for (int y = 0; y < height; y++)
            {
                bool stop = false;
                for (int x = 0; x < width; x++)
                {
                    //Check cancellation token:
                    if (cancellationToken.IsCancellationRequested)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    //Do our work
                    if (pos == count)
                    {
                        stop = true;
                        break;
                    }

                    SKColor pixel = inputImage.GetPixel(x, y);

                    int[] vals = new int[] { pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue };

                    for (int v = 0; v < vals.Length; v++)
                    {
                        if (!UseAlpha && v == 0) continue;

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

                    //pixel = Color.FromArgb(vals[0], vals[1], vals[2], vals[3]);

                    outputImage.SetPixel(x, y, pixel);

                    //Report on progress:
                    if (progress != null)
                    {
                        float percent = pos / (float)count;
                        progress.Report(percent);
                    }
                }

                if (stop) break;
            }
            outputImage.Encode(destination, SKEncodedImageFormat.Png, 100);
            //outputImage.Save(destination, ImageFormat.Png);
            return Task.CompletedTask;
        }
        
        public override Task EjectAsync(Stream source, Stream destination, IProgress<float> progress=null, CancellationToken cancellationToken=default)
        {
            //Prepare the image
            //var inputImage = (Bitmap)Image.FromStream(source);
            SKBitmap inputImage =SKBitmap.Decode(source);

            int height = inputImage.Height;
            int width = inputImage.Width;

            //Prepare the writing stage
            int count = (width * height) * LsbDepth * (UseAlpha ? 4 : 3);
            BitArray message = new BitArray(count, false);
            int pos = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //Check cancellation token:
                    if (cancellationToken.IsCancellationRequested)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    SKColor pixel = inputImage.GetPixel(x, y);

                    byte[] vals = UseAlpha ? new byte[] { pixel.Alpha, pixel.Red, pixel.Green, pixel.Blue }
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

                    //Report on progress:
                    if (progress != null)
                    {
                        progress.Report(pos / (float)count);
                    }
                }
            }

            //Write the result
            byte[] bytes = message.ToBytes();
            List<byte> result;

            //Return the file cause there is no need to find the EOF
            if (string.IsNullOrEmpty(EOF))
            {
                result = new List<byte>(bytes);
            }
            else
            {
                result = new List<byte>();
                bool success = false;

                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] == EOFBytes[0])
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