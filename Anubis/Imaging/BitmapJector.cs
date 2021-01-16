using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Threading.Tasks;

namespace Thismaker.Anubis.Imaging
{
    /// <summary>
    /// A Jector that allows for writing data into and from <see cref="Bitmap"/> 
    /// image files.
    /// </summary>
    public class BitmapJector : IJector<Bitmap>
    {
        /// <summary>
        /// The string that will be used to signify an End of File, if not assigned, 
        /// a default value will be used.
        /// When calling <see cref="Inject(Bitmap, byte[])"/> the string is added at the
        /// end of the file before infusing the data
        /// During <see cref="Eject(Bitmap)"/> the string is checked at the end of the obtained data, 
        /// the returned data is truncated at where the EOF was found.
        /// If the value is null, the EOF will not be added at the end nor will it be checked.
        /// </summary>
        public string EOF { get; set; } = "#$%-";

        /// <summary>
        /// The depth of the Least Significant Bit, increasing the value may allow storing bigger files,
        /// but may lead to noticable artifacts in the produced data. Value cannot be greater than 8
        /// </summary>
        public int LsbDepth { get; set; } = 2;

        /// <summary>
        /// When <see cref="true"/>, the Alpha Channel will also be written to, 
        /// and read from,
        /// allowing for writing more data into the image. 
        /// However, the resultant image may have artifacts.
        /// </summary>
        public bool UseAlpha { get; set; } = false;

        /// <summary>
        /// The EOF as a byte array, using UTF8 Encoding
        /// </summary>
        private byte[] Sign
        {
            get { return Encoding.UTF8.GetBytes(EOF); }
        }

        /// <summary>
        /// When <see cref="true"/> throws <see cref="InvalidOperationException"/> 
        /// when the EOF cannot be written or was not read.
        /// </summary>
        public bool EnsureSuccess { get; set; } = false;

        public byte[] InjectBytes(byte[] input, byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] EjectBytes(byte[] input)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Injects byte array into an image, writing the EOF at the very end. 
        /// The image must be saved as a PNG to preserve the data within, as other formarts are lossy
        /// therefore may lose the data stored
        /// </summary>
        /// <param name="input">The image in which the data is to be written</param>
        /// <param name="data">The byte array to write inside the iamge</param>
        /// <returns></returns>
        public Bitmap Inject(Bitmap input, byte[] data)
        {
            var output = input.Clone(new Rectangle(0, 0, input.Width, input.Height), input.PixelFormat);
            var pos = 0;

            var ls_data = new List<byte>(data);
            
            if (!string.IsNullOrEmpty(EOF))
            {
                ls_data.AddRange(Sign);
            }

            var message = new BitArray(ls_data.ToArray());

            var count = message.Count;

            //check for ensure success:
            if (EnsureSuccess)
            {
                var maxWritable = (UseAlpha ? 4 : 3) * LsbDepth * input.Width * input.Height;
                if (count>maxWritable) 
                    throw new InvalidOperationException("There is not enough room in the picture to write the file");
            }

            for (int y=0; y<output.Height; y++)
            {
                bool stop = false;
                for(int x = 0; x < output.Width; x++)
                {
                    if (pos == count)
                    {
                        stop = true;
                        break;
                    }

                    var pixel = input.GetPixel(x, y);
                    
                    int[] vals=UseAlpha? new int[] { pixel.A, pixel.R, pixel.G, pixel.B }
                    :  new int[] { pixel.R, pixel.G, pixel.B }; ;
                   

                    for(int v = 0; v < vals.Length;v++)
                    {
                        var bitArray = new BitArray(new[] { vals[v] });
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

                        vals[v] = (int)bitArray.ToInt();
                        if (stop) break;
                    }

                    pixel = UseAlpha? Color.FromArgb(vals[0], vals[1], vals[2], vals[3]) 
                        :Color.FromArgb(vals[0], vals[1], vals[2]);

                    output.SetPixel(x, y, pixel);
                }

                if (stop) break;
            }

            return output;
        }

        public Task<Bitmap> InjectAsync(Bitmap input, byte[] data)
        {
            return Task.Run(()=>Inject(input, data));
        }

        public Task<byte[]> EjectAsync(Bitmap input)
        {
            return Task.Run(() => Eject(input));
        }

        /// <summary>
        /// Extracts a byte array of data from the provided image file. If an EOF was specified, 
        /// the function probes the data found for the EOF and returns the truncated value.
        /// Otherwise, it returns the entire byte array found in the file.
        /// </summary>
        /// <param name="input">The image in which the data to be extracted resides</param>
        /// <returns></returns>
        public byte[] Eject(Bitmap input)
        {
            int count = (input.Width * input.Height) * LsbDepth * (UseAlpha ? 4 : 3);
            var message = new BitArray(count, false);
            var pos = 0;
            //get the first pixel data:

            for (int y = 0; y < input.Height; y++)
            {
                for (int x = 0; x < input.Width; x++)
                {
                    var pixel = input.GetPixel(x, y);

                    int[] vals = UseAlpha ? new int[] { pixel.A, pixel.R, pixel.G, pixel.B }
                    : new int[] { pixel.R, pixel.G, pixel.B }; ;

                    foreach (var val in vals)
                    {
                        var bitArray = new BitArray(new[] { val });
                        for (int i = 0; i < LsbDepth; i++)
                        {
                            message[pos] = bitArray[i];
                            pos++;
                        }
                    }
                }
            }
            var bytes= message.ToBytes();

            //Return the file cause there is no need to find the EOF
            if (string.IsNullOrEmpty(EOF)) return bytes;

            var result = new List<byte>();

            var success = false;
            
            for(int i = 0; i < bytes.Length; i++)
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

            if(EnsureSuccess && !success)
                throw new InvalidOperationException("Failed to locate the specified EOF");

            return result.ToArray();
        }

        bool IsSignature(byte[] input, int index)
        {
            try
            {
                foreach (var b in Sign)
                {

                    if (b != input[index]) return false;
                    index++;
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
