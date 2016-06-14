using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace wmgCMS
{
    /// <summary>
    /// Provides helper methods for imaging
    /// </summary>
    public static class ImageHelper
    {
        private static byte[] pngiconheader =
                        new byte[] { 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 24, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static Icon PngIconFromImage(Image img, Size s)
        {
            using (Bitmap bmp = new Bitmap(img, s))
            {
                byte[] png;
                using (System.IO.MemoryStream fs = new System.IO.MemoryStream())
                {
                    bmp.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                    fs.Position = 0;
                    png = fs.ToArray();
                }

                using (System.IO.MemoryStream fs = new System.IO.MemoryStream())
                {
                    if (s.Width >= 256 || s.Height >= 256) { s.Width = 256; s.Height = 256; }
                    pngiconheader[6] = (byte)s.Width;
                    pngiconheader[7] = (byte)s.Width;
                    pngiconheader[14] = (byte)(png.Length & 255);
                    pngiconheader[15] = (byte)(png.Length / 256);
                    pngiconheader[18] = (byte)(pngiconheader.Length);

                    fs.Write(pngiconheader, 0, pngiconheader.Length);
                    fs.Write(png, 0, png.Length);
                    fs.Position = 0;
                    return new Icon(fs);
                }
            }
        }
    }
}
