using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Enigma
{
    public static class Black
    {
        public static void GetThismaker(Stream stream)
        {
            var bmp = (Bitmap)Image.FromStream(stream);

            for(int x = 0; x < bmp.Width;x++)
            {
                for(int y = 0; y < bmp.Height; y++)
                {
                    var color = bmp.GetPixel(x, y);

  
                }
            }

            
        }
    }
}
